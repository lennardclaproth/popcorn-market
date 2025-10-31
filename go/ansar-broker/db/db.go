package db

import (
	"fmt"

	"github.com/jmoiron/sqlx"

	// _ "github.com/lib/pq"
	_ "modernc.org/sqlite"
)

type ConnectionType string

const (
	Sqlite   ConnectionType = "sqlite"
	Postgres ConnectionType = "postgres"
)

func NewDB(connStr string, connType ConnectionType) *sqlx.DB {
	db, err := sqlx.Open(string(connType), connStr)
	if err != nil {
		panic(fmt.Errorf("db: failed to open connection to database: %w", err))
	}

	return db
}
