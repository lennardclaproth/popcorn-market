package db

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/errorx"
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
	var schemaAccounts []Account

	query := fmt.Sprintf(
		`SELECT * 
		FROM %s 
		WHERE id = ?`,
		TableAccounts,
	)

	err := as.db.SelectContext(ctx, &schemaAccounts, query, id.String())
	if err != nil {
		return nil, fmt.Errorf("get account by id %s: %w", id, err)
	}

	accounts := make([]account.Account, 0, len(schemaAccounts))

	for _, sa := range schemaAccounts {
		da, err := ToDomainAccount(sa)
		if err != nil {
			return nil, fmt.Errorf("convert schema account %s: %w", sa.ID, err)
		}
		accounts = append(accounts, da)
	}

	return &accounts[0], nil
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
	schemaAccount := ToSchemaAccount(*account)

	query := fmt.Sprintf(`
		UPDATE %s
		SET
			balance = ?,
			status = ?,
			total_value = ?,
			unrealized_pnl = ?,
			is_active = ?
		WHERE id = ?
	`, TableAccounts)

	_, err := as.db.ExecContext(
		ctx,
		query,
		schemaAccount.Balance,
		schemaAccount.Status,
		schemaAccount.TotalValue,
		schemaAccount.UnrealizedPnL,
		schemaAccount.IsActive,
		schemaAccount.ID,
	)

	if err != nil {
		return errorx.Trace(fmt.Errorf("update account failed: %w", err))
	}

	return nil
}
