package persistence

import (
	"fmt"

	"github.com/google/uuid"
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/internal/modules/account"
)

type accountRepository struct {
	db *sqlx.DB
}

func NewAccountRepositorySqlx(db *sqlx.DB) account.Repository {
	return &accountRepository{db: db}
}

// Create implements domain.AccountRepository.
func (a *accountRepository) Create(account *account.Account) error {
	sa := SchemaAccount(*account)
	query := fmt.Sprintf(`INSERT INTO %s (
		id, user_id, account_number, balance, status, total_value, unrealized_pnl, realized_pnl, created_at, updated_at, deleted_at) VALUES (
			:id, :user_id, :account_number, :balance, :status, :total_value, :unrealized_pnl, :realized_pnl, :created_at, :updated_at, :deleted_at
		);`, TableAccounts)

	_, err := a.db.NamedExec(query, sa)

	return err
}

// Delete implements domain.AccountRepository.
func (a *accountRepository) Delete(id uuid.UUID) error {
	panic("unimplemented")
}

// GetByID implements domain.AccountRepository.
func (a *accountRepository) GetByID(id uuid.UUID) (*account.Account, error) {
	panic("unimplemented")
}

// GetByUserID implements domain.AccountRepository.
func (a *accountRepository) GetByUserID(userID uuid.UUID) (*account.Account, error) {
	panic("unimplemented")
}

// Update implements domain.AccountRepository.
func (a *accountRepository) Update(account *account.Account) error {
	panic("unimplemented")
}
