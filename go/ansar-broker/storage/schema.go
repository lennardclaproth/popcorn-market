package storage

import (
	"database/sql"
	"time"

	"github.com/google/uuid"
)

const (
	SchemaMain    = "ansar"
	TableUsers    = "users"
	TableAccounts = "accounts"
	TableHoldings = "holdings"
	TableOrders   = "orders"
)

type AccountStatus int

const (
	AccountStatusActive AccountStatus = iota
	AccountStatusPending
	AccountStatusInactive
	AccountStatusSuspended
	AccountStatusClosed
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
	IsActive      bool          `db:"is_active"`
	UpdatedAt     time.Time     `db:"updated_at"`
	DeletedAt     sql.NullTime  `db:"deleted_at"`
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
	OrderStatusPending OrderStatus = iota
	OrderStatusCompleted
	OrderStatusCanceled
)

type Order struct {
	ID            uuid.UUID   `db:"id"`
	OrderId       string      `db:"order_id"`
	PlacedAt      time.Time   `db:"placed_at"`
	AccountID     uuid.UUID   `db:"account_id"`
	AccountNumber string      `db:"account_number"`
	Ticker        string      `db:"ticker"`
	Quantity      int         `db:"quantity"`
	Price         float64     `db:"price"`
	Side          OrderSide   `db:"order_side"`
	Type          OrderType   `db:"order_type"`
	Status        OrderStatus `db:"order_status"`
	UpdatedAt     time.Time   `db:"updated_at"`
}
