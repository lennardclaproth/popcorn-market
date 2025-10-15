package db

import (
	"time"

	"github.com/google/uuid"
)

const (
	SchemaMain    = "ansar"
	TableUsers    = "users"
	TableAccounts = "accounts"
	TableHoldings = "holdings"
)

type AccountStatus string

const (
	AccountStatusActive    AccountStatus = "active"
	AccountStatusPending   AccountStatus = "pending"
	AccountStatusInactive  AccountStatus = "inactive"
	AccountStatusSuspended AccountStatus = "suspended"
	AccountStatusClosed    AccountStatus = "closed"
)

type User struct {
	ID          uuid.UUID `db:"id"`
	Email       string    `db:"email"`
	FirstName   string    `db:"firstname"`
	LastName    string    `db:"lastname"`
	DateOfBirth time.Time `db:"date_of_birth"`
	Password    string    `db:"password"`
	CreatedAt   time.Time `db:"created_at"`
	Street      string    `db:"street"`
	City        string    `db:"city"`
	State       string    `db:"state"`
	ZipCode     string    `db:"zip_code"`
	Country     string    `db:"country"`
}

type Account struct {
	ID            uuid.UUID     `db:"id"`
	UserID        uuid.UUID     `db:"user_id"`
	Number        string        `db:"account_number"`
	Balance       float64       `db:"balance"`
	Status        AccountStatus `db:"status"`
	TotalValue    float64       `db:"total_value"`
	UnrealizedPnL float64       `db:"unrealized_pnl"`
	RealizedPnL   float64       `db:"realized_pnl"`
	Holdings      []Holding     `db:"holdings"`
	OpenedDate    time.Time     `db:"opened_at"`
}

type Holding struct {
	Symbol    string    `db:"symbol"`
	AccountID uuid.UUID `db:"account_id"`
	Quantity  float64   `db:"quantity"`
	AvgPrice  float64   `db:"avg_price"`
	Security  Security  `db:"security"`
}

type Security struct {
	Symbol string `db:"symbol"`
	Name   string `db:"name"`
}

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
