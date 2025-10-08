package order

import (
	"github.com/google/uuid"
)

type OrderSide string
const (
	OrderSideBuy  OrderSide = "buy"
	OrderSideSell OrderSide = "sell"
)

type OrderType string
const (
	OrderTypeMarket OrderType = "market"
	OrderTypeLimit  OrderType = "limit"
)

type OrderStatus string
const (
	OrderStatusPending   OrderStatus = "pending"
	OrderStatusCompleted OrderStatus = "completed"
	OrderStatusCanceled  OrderStatus = "canceled"
)

type Order struct {
	ID        uuid.UUID
	AccountID uuid.UUID
	Ticker	string
	Quantity  float64
	Price     float64
	Side      OrderSide
	Type      OrderType
	Status    OrderStatus
}

func NewOrder(accountID uuid.UUID, ticker string, quantity, price float64, side OrderSide, orderType OrderType) *Order {
	return &Order{
		ID:        uuid.New(),
		AccountID: accountID,
		Ticker:   ticker,
		Quantity: quantity,
		Price:    price,
		Side:     side,
		Type:     orderType,
		Status:   OrderStatusPending,
	}
}
