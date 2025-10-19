package account

import (
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/crypto"
	"github.com/lennardclaproth/ansar-broker/internal/security"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type AccountStatus int

const (
	AccountStatusActive AccountStatus = iota
	AccountStatusPending
	AccountStatusInactive
	AccountStatusSuspended
	AccountStatusClosed
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
	IsActive   bool
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

type AccountInfo struct {
	ID         uuid.UUID
	Number     string
	Balance    float64
	OpenedDate time.Time
	IsActive   bool
	Status     int
}

type Service struct {
	userHandler UserHandler
	store       Store
	logger      logging.Logger
}

var (
	ErrUserCannotOpenAccount = errors.New("User cannot open account")
)

type UserHandler interface {
	CanOpenAccount(ctx context.Context, uid uuid.UUID) (bool, error)
}

type Store interface {
	GetByID(ctx context.Context, id uuid.UUID) (*Account, error)
	GetByUserID(ctx context.Context, userID uuid.UUID) (*[]Account, error)
	Create(ctx context.Context, account *Account) error
	Update(ctx context.Context, account *Account) error
	Delete(ctx context.Context, id uuid.UUID) error
}

func NewService(store Store, log logging.Logger, userHandler UserHandler) *Service {
	return &Service{
		userHandler: userHandler,
		store:       store,
		logger:      log,
	}
}

type OpenAccountCommand struct {
	UserID         uuid.UUID
	InitialBalance float64
}

func (s *Service) Open(ctx context.Context, cmd OpenAccountCommand) (uuid.UUID, error) {
	// Check if there are already existing accounts for this user
	// If there is an existing account for this user the newly created
	// account will not be the active account. The user would need
	// to do this manually.
	canOpenAccount, err := s.userHandler.CanOpenAccount(ctx, cmd.UserID)

	if err != nil {
		return uuid.Nil, err
	}

	if !canOpenAccount {
		return uuid.Nil, fmt.Errorf("%w: user %s cannot open account",
			ErrUserCannotOpenAccount, cmd.UserID)
	}

	accounts, err := s.store.GetByUserID(ctx, cmd.UserID)

	if err != nil {
		return uuid.Nil, err
	}

	hasActiveAccount := false
	for _, acc := range *accounts {
		if acc.IsActive {
			hasActiveAccount = true
			break
		}
	}

	prefix := "ANSR"
	number := crypto.RandomString(12)

	a := &Account{
		ID:      uuid.New(),
		UserID:  cmd.UserID,
		Balance: cmd.InitialBalance,
		Number:  fmt.Sprintf("%s-%s", prefix, number),
		Portfolio: Portfolio{
			Holdings:      []Holding{},
			TotalValue:    0,
			UnrealizedPnL: 0,
			RealizedPnL:   0,
		},
		Status:     AccountStatusActive,
		Orders:     []uuid.UUID{},
		OpenedDate: time.Now(),
		IsActive:   !hasActiveAccount,
	}

	err = s.store.Create(ctx, a)
	if err != nil {
		return uuid.Nil, err
	}

	return a.ID, nil
}

func (s *Service) CanPlaceOrder(ctx context.Context, accId uuid.UUID, price float64) (bool, error) {
	return true, nil
}

func (s *Service) GetAccountInfo(ctx context.Context, accId uuid.UUID) (AccountInfo, error) {
	acc, err := s.store.GetByID(ctx, accId)
	if err != nil {
		return AccountInfo{}, err
	}

	return AccountInfo{
		ID:         acc.ID,
		Number:     acc.Number,
		Balance:    acc.Balance,
		Status:     int(acc.Status),
		IsActive:   acc.IsActive,
		OpenedDate: acc.OpenedDate,
	}, nil
}

func (s *Service) DeductFunds(ctx context.Context, accId uuid.UUID, price float64) error {
	return nil
}
