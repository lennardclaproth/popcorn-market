package account

import (
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/errorx"
	"github.com/lennardclaproth/ansar-broker/internal/security"
	"github.com/lennardclaproth/ansar-broker/logging"

	"crypto/rand"
	"math/big"
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

type Accounts struct {
	userHandler UserHandler
	store       Store
	logger      logging.Logger
}

type WatchList struct {
	Securities []security.Security
}

var (
	ErrUserCannotOpenAccount = errors.New("user cannot open account")
	ErrInactiveAccount       = errors.New("account is not active")
	ErrInsufficientFunds     = errors.New("account has insufficient funds")
)

type UserHandler interface {
	CanOpenAccount(ctx context.Context, uid uuid.UUID) (bool, error)
}

const charset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

func RandomString(n int) string {
	b := make([]byte, n)
	for i := range b {
		num, _ := rand.Int(rand.Reader, big.NewInt(int64(len(charset))))
		b[i] = charset[num.Int64()]
	}
	return string(b)
}

type Store interface {
	GetByID(ctx context.Context, id uuid.UUID) (*Account, error)
	GetWithPortfolio(ctx context.Context, id uuid.UUID) (*Account, error)
	GetByUserID(ctx context.Context, userID uuid.UUID) ([]Account, error)
	Create(ctx context.Context, account *Account) error
	Update(ctx context.Context, account *Account) error
	Delete(ctx context.Context, id uuid.UUID) error
}

func NewService(store Store, log logging.Logger, userHandler UserHandler) *Accounts {
	return &Accounts{
		userHandler: userHandler,
		store:       store,
		logger:      log,
	}
}

type OpenAccountCommand struct {
	UserID         uuid.UUID
	InitialBalance float64
}

func (s *Accounts) Open(ctx context.Context, cmd OpenAccountCommand) (string, error) {
	// Check if there are already existing accounts for this user
	// If there is an existing account for this user the newly created
	// account will not be the active account. The user would need
	// to do this manually.
	canOpenAccount, err := s.userHandler.CanOpenAccount(ctx, cmd.UserID)

	if err != nil {
		return "", errorx.Trace(fmt.Errorf("open: failed to execute: %w", err))
	}

	if !canOpenAccount {
		return "", errorx.Trace(fmt.Errorf("%w: user %s cannot open account",
			ErrUserCannotOpenAccount, cmd.UserID))
	}

	accounts, err := s.store.GetByUserID(ctx, cmd.UserID)

	if err != nil {
		return "", errorx.Trace(fmt.Errorf("open: failed to execute: %w", err))
	}

	hasActiveAccount := false
	for _, acc := range accounts {
		if acc.IsActive {
			hasActiveAccount = true
			break
		}
	}

	prefix := "ANSR"
	number := RandomString(12)

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
		return "", errorx.Trace(fmt.Errorf("open: failed to execute: %w", err))
	}

	return a.Number, nil
}

func (s *Accounts) CanPlaceOrder(ctx context.Context, accId uuid.UUID, price float64) (bool, error) {
	return true, nil
}

// getAccountInfo returns the info of an account.
func (s *Accounts) GetAccountInfo(ctx context.Context, accId uuid.UUID) (AccountInfo, error) {
	// access the store to get the account by the account ID
	acc, err := s.store.GetByID(ctx, accId)
	if err != nil {
		return AccountInfo{}, errorx.Trace(fmt.Errorf("getAccountInfo: failed to execute: %w", err))
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

// getAccountInfo returns the info of an account.
func (s *Accounts) GetActiveAccount(ctx context.Context, uId uuid.UUID) (AccountInfo, error) {
	// access the store to get the account by the account ID
	accs, err := s.store.GetByUserID(ctx, uId)
	if err != nil {
		return AccountInfo{}, errorx.Trace(fmt.Errorf("getAccountInfo: failed to execute: %w", err))
	}

	// Find the active account
	for _, a := range accs {
		if a.IsActive {
			return AccountInfo{
				ID:         a.ID,
				Number:     a.Number,
				Balance:    a.Balance,
				Status:     int(a.Status),
				IsActive:   a.IsActive,
				OpenedDate: a.OpenedDate,
			}, nil
		}
	}

	// No active account found
	return AccountInfo{}, errorx.Trace(fmt.Errorf("getAccountInfo: no active account found for user %s", uId))
}

func (s *Accounts) DeductFunds(ctx context.Context, accId uuid.UUID, price float64) error {
	acc, err := s.store.GetByID(ctx, accId)

	if err != nil {
		return errorx.Trace(fmt.Errorf("deductFunds: failed to get account by id: %w", err))
	}

	if !acc.IsActive {
		return ErrInactiveAccount
	}

	if acc.Balance < price {
		return ErrInsufficientFunds
	}

	acc.Balance -= price
	err = s.store.Update(ctx, acc)

	if err != nil {
		return errorx.Trace(fmt.Errorf("deductFunds: failed to persist changes in the store: %w", err))
	}

	return nil
}

func (s *Accounts) Refund(ctx context.Context, accId uuid.UUID, price float64) error {
	acc, err := s.store.GetByID(ctx, accId)

	if err != nil {
		return errorx.Trace(fmt.Errorf("refund: failed to get account by id: %w", err))
	}

	acc.Balance += price
	err = s.store.Update(ctx, acc)
	if err != nil {
		return errorx.Trace(fmt.Errorf("refund: failed to persist changes in the store: %w", err))
	}

	return err
}

func (s *Accounts) AddFunds(ctx context.Context, accId uuid.UUID, price float64) error {
	acc, err := s.store.GetByID(ctx, accId)

	if err != nil {
		return errorx.Trace(fmt.Errorf("addfunds: failed to get account by id: %w", err))
	}

	acc.Balance += price
	err = s.store.Update(ctx, acc)
	if err != nil {
		return errorx.Trace(fmt.Errorf("addfunds: failed to persist changes in the store: %w", err))
	}

	return err
}

func (s *Accounts) AddToHolding(ctx context.Context, symbol string, quantity int, price float64, accId uuid.UUID) {
	a, err := s.store.GetWithPortfolio(ctx, accId)

	if err != nil {

	}

	for _, h := range(a.Portfolio.Holdings){
		if h.Security.Symbol != symbol {
			continue
		}
	}
}

func (s *Accounts) RemoveFromHolding(ctx context.Context, symbol string, quanity int, accId uuid.UUID) {
	
}

func (s *Accounts) GetPortfolio(ctx context.Context, accId uuid.UUID) {

}