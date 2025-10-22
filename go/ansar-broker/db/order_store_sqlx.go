package db

import (
	"context"
	"fmt"

	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/internal/order"
)

type OrderStore struct {
	db *sqlx.DB
}

func NewOrderStoreSqlx(db *sqlx.DB) *OrderStore {
	return &OrderStore{
		db: db,
	}
}

func (os *OrderStore) Create(ctx context.Context, o *order.Order) error {
	schemaOrder := ToSchemaOrder(*o)
	query := fmt.Sprintf(`INSERT INTO %s (
		id,
		order_id,
		account_id,
		ticker,
		quantity,
		price,
		order_side,
		order_type,
		order_status,
		updated_at
	) VALUES (
		:id,
		:order_id,
		:account_id,
		:ticker,
		:quantity,
		:price,
		:order_side,
		:order_type,
		:order_status,
		:updated_at
	)`, TableOrders)

	_, err := os.db.NamedExec(query, schemaOrder)
	return err
}

func (os *OrderStore) UpdateOrderPlaced(ctx context.Context, o *order.Order) error {
	schema := ToSchemaOrder(*o)
	query := fmt.Sprintf(`UPDATE %s SET order_status=?, updated_at=? WHERE id=?`, TableOrders)
	_, err := os.db.ExecContext(ctx, query,
		schema.Status, schema.UpdatedAt, schema.ID)
	return err
}
