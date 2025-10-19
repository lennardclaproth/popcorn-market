package db

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/internal/account"
)

type accountStore struct {
	db *sqlx.DB
}

func NewAccountStoreSqlx(db *sqlx.DB) account.Store {
	return &accountStore{db: db}
}

func (as *accountStore) Create(ctx context.Context, account *account.Account) error {
	sa := ToSchemaAccount(*account)
	query := fmt.Sprintf(
		`INSERT INTO %s (id, user_id, account_number, balance, status, total_value, unrealized_pnl, realized_pnl, opened_at, is_active, updated_at) 
		VALUES (
			:id, :user_id, :account_number, :balance, :status, :total_value, :unrealized_pnl, :realized_pnl, :opened_at, :is_active, :updated_at
		);`, TableAccounts)

	_, err := as.db.NamedExec(query, sa)

	return err
}

func (as *accountStore) Delete(ctx context.Context, id uuid.UUID) error {
	panic("unimplemented")
}

func (as *accountStore) GetByID(ctx context.Context, id uuid.UUID) (*account.Account, error) {
	panic("unimplemented")
}

func (as *accountStore) GetByUserID(ctx context.Context, userID uuid.UUID) (*[]account.Account, error) {
	var schemaAccounts []Account

	query := fmt.Sprintf(`
		SELECT 
			id, 
			user_id, 
			account_number, 
			balance, 
			status,
			total_value,
			unrealized_pnl,
			realized_pnl,
			is_active, 
			opened_at
		FROM %s
		WHERE user_id = ?
	`, TableAccounts)

	if err := as.db.SelectContext(ctx, &schemaAccounts, query, userID.String()); err != nil {
		return nil, fmt.Errorf("get accounts by user %s: %w", userID, err)
	}

	accounts := make([]account.Account, 0, len(schemaAccounts))

	for _, acc := range schemaAccounts {
		dAcc, err := ToDomainAccount(acc)
		if err != nil {
			return nil, fmt.Errorf("convert schema account %s: %w", acc.ID, err)
		}
		accounts = append(accounts, dAcc)
	}

	return &accounts, nil
}

func (as *accountStore) Update(ctx context.Context, account *account.Account) error {
	panic("unimplemented")
}
