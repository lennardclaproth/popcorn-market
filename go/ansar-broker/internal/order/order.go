package order

import (
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/errorx"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type OrderSide int

const (
	OrderSideBuy OrderSide = iota
	OrderSideSell
)

type OrderType int

const (
	OrderTypeMarket OrderType = iota
	OrderTypeLimit
)

type OrderStatus int

const (
	OrderStatusNew OrderStatus = iota
	OrderStatusPending
	OrderStatusCompleted
	OrderStatusCanceled
	OrderStatusError
)

type Order struct {
	ID        uuid.UUID
	OrderId   string
	PlacedAt  time.Time
	AccountID uuid.UUID
	Ticker    string
	Quantity  int
	Price     float64
	Side      OrderSide
	Type      OrderType
	Status    OrderStatus
}

type Service struct {
	store    Store
	log      logging.Logger
	accounts AccountHandler
	exchange ExchangeHandler
}

type ExchangeHandler interface {
	PlaceOrder(o *Order) (string, error)
}

type AccountHandler interface {
	GetAccountInfo(ctx context.Context, accountId uuid.UUID) (account.AccountInfo, error)
	DeductFunds(ctx context.Context, accountId uuid.UUID, price float64) error
	Refund(ctx context.Context, accountId uuid.UUID, price float64) error
}

type Store interface {
	Create(ctx context.Context, o *Order) error
	UpdateOrderPlaced(ctx context.Context, o *Order) error
	Update(ctx context.Context, o *Order) error
}

func NewService(s Store, log logging.Logger, ah AccountHandler, eh ExchangeHandler) *Service {
	os := &Service{
		store:    s,
		log:      log,
		accounts: ah,
		exchange: eh,
	}

	return os
}

type PlaceOrderCommand struct {
	AccountID     uuid.UUID
	AccountNumber string
	Ticker        string
	Quantity      int
	Price         float64
	Side          OrderSide
	Type          OrderType
}

var (
	ErrAccountCannotPlaceOrder = errors.New("account cannot place order")
	ErrInvalidPrice            = errors.New("invalid order price")
	ErrExchangeFailure         = errors.New("failed to place order on exchange")
)

// PlaceOrder creates a new order and adds it to the database than sends it to the correct exchange to
// be processed.
func (s *Service) PlaceOrder(ctx context.Context, cmd PlaceOrderCommand) (string, error) {
	// deduct funds first to check if the account is able
	// to place the order.
	totalprice := cmd.Price * float64(cmd.Quantity)
	err := s.accounts.DeductFunds(ctx, cmd.AccountID, totalprice)
	if err != nil {
		return "", errorx.Trace(fmt.Errorf("placeOrder: failed to deduct funds: %w", err))
	}

	// Create a new order and store it in the database to make sure that
	// it can be handled correctly if an errors occur. Deduct the funds
	// when the order has been saved.
	o := &Order{
		ID:        uuid.New(),
		AccountID: cmd.AccountID,
		PlacedAt:  time.Now(),
		Ticker:    cmd.Ticker,
		Quantity:  cmd.Quantity,
		Price:     cmd.Price,
		Side:      cmd.Side,
		Type:      cmd.Type,
		Status:    OrderStatusNew,
	}
	err = s.store.Create(ctx, o)
	if err != nil {
		err = errorx.Trace(fmt.Errorf("placeOrder: failed to create order: %w", err))
		refundErr := s.accounts.Refund(ctx, cmd.AccountID, totalprice)
		err = errorx.Trace(fmt.Errorf("placeOrder: failed to refund : %w: %w", refundErr, err))
		return "", err
	}

	// place the order on the exchange and make sure to set the orderId
	// of the order to the orderId that is returned from the exchange so
	// we can map it back when the exchange publishes updates on the order.
	oid, err := s.exchange.PlaceOrder(o)
	if err != nil {
		err = errorx.Trace(fmt.Errorf("placeOrder: failed to place order: %w", err))
		refundErr := s.accounts.Refund(ctx, cmd.AccountID, totalprice)
		if refundErr != nil {
			err = errorx.Trace(fmt.Errorf("placeOrder: failed to refund : %w: %w", refundErr, err))
		}
		o.Status = OrderStatusError
		orderErr := s.store.Update(ctx, o)
		if orderErr != nil {
			err = errorx.Trace(fmt.Errorf("placeOrder: failed to update order status to error: %w: %w", orderErr, err))
		}
		return "", err
	}
	o.OrderId = oid
	o.Status = OrderStatusPending
	err = s.store.UpdateOrderPlaced(ctx, o)
	if err != nil {
		return "", err
	}

	return o.OrderId, nil
}
