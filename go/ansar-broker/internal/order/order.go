package order

import (
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/google/uuid"
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
	// PlaceOrder tries to place an order on the exchange.
	PlaceOrder(o *Order) (string, error)
}

type AccountHandler interface {
	// CanPlaceOrder checkes if the account is able to place an order for that
	// account.
	GetAccountInfo(ctx context.Context, accountId uuid.UUID) (account.AccountInfo, error)
	DeductFunds(ctx context.Context, accountId uuid.UUID, price float64) error
}

type Store interface {
	Create(ctx context.Context, o *Order) error
	UpdateOrderPlaced(ctx context.Context, o *Order) error
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
	// canPlaceOrder, err := s.accounts.CanPlaceOrder(ctx, cmd.AccountID, cmd.Price)

	acc, err := s.accounts.GetAccountInfo(ctx, cmd.AccountID)

	if err != nil {
		return "", err
	}

	if acc.Balance < cmd.Price || !acc.IsActive {
		return "", fmt.Errorf("%w: account %s cannot place order at price %.2f",
			ErrAccountCannotPlaceOrder, cmd.AccountID, cmd.Price)
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
		return "", err
	}
	s.accounts.DeductFunds(ctx, acc.ID, o.Price)

	// place the order on the exchange and make sure to set the orderId
	// of the order to the orderId that is returned from the exchange so
	// we can map it back when the exchange publishes updates on the order.
	oid, err := s.exchange.PlaceOrder(o)
	if err != nil {
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
