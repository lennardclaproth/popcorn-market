package account

import (
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/internal/security"
)

type AccountStatus string

const (
	AccountStatusActive    AccountStatus = "active"
	AccountStatusPending   AccountStatus = "pending"
	AccountStatusInactive  AccountStatus = "inactive"
	AccountStatusSuspended AccountStatus = "suspended"
	AccountStatusClosed    AccountStatus = "closed"
)

type Account struct {
	ID         uuid.UUID
	UserID     uuid.UUID
	Number     string
	Balance    float64
	Portfolio  Portfolio
	Status     AccountStatus
	Orders     []uuid.UUID
	OpenedDate time.Time
}

type Portfolio struct {
	Holdings      []Holding
	TotalValue    float64
	UnrealizedPnL float64
	RealizedPnL   float64
}

type Holding struct {
	Security security.Security
	Quantity float64
	AvgPrice float64
}

type AccountRepository interface {
	GetByID(id uuid.UUID) (*Account, error)
	GetByUserID(userID uuid.UUID) (*Account, error)
	Create(account *Account) error
	Update(account *Account) error
	Delete(id uuid.UUID) error
}

func NewAccount(userID uuid.UUID, initialBalance float64) *Account {
	return &Account{
		ID:      uuid.New(),
		UserID:  userID,
		Balance: initialBalance,
		Number: "",
		Portfolio: Portfolio{
			Holdings:      []Holding{},
			TotalValue:    0,
			UnrealizedPnL: 0,
			RealizedPnL:   0,
		},
		Status: AccountStatusActive,
		Orders: []uuid.UUID{},
		OpenedDate: time.Now(),
	}
}
