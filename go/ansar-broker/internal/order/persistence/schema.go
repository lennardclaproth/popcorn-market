package persistence

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
	ID        uuid.UUID `gorm:"type:uuid;primaryKey"`
	AccountID uuid.UUID `gorm:"type:uuid;index;not null"`
	Ticker    string
	Quantity  float64
	Price     float64
	Side      OrderSide
	Type      OrderType
	Status    OrderStatus
}

