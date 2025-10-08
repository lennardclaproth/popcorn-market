package persistence

import (
	"time"

	"github.com/google/uuid"
	security "github.com/lennardclaproth/ansar-broker/internal/security/persistence"
)

type AccountStatus string

const (
	AccountStatusActive    AccountStatus = "active"
	AccountStatusPending   AccountStatus = "pending"
	AccountStatusInactive  AccountStatus = "inactive"
	AccountStatusSuspended AccountStatus = "suspended"
	AccountStatusClosed    AccountStatus = "closed"
)

const (
	SchemaMain    = "ansar"
	TableAccounts = "accounts"
	TableHoldings = "holdings"
)

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
	Symbol    string            `db:"symbol"`
	AccountID uuid.UUID         `db:"account_id"`
	Quantity  float64           `db:"quantity"`
	AvgPrice  float64           `db:"avg_price"`
	Security  security.Security `db:"security"`
}
