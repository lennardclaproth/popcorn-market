package db

import (
	"fmt"

	"github.com/jmoiron/sqlx"

	// _ "github.com/lib/pq"
	"go.elastic.co/apm/module/apmsql/v2"
	_ "go.elastic.co/apm/module/apmsql/v2/sqlite3"
	_ "modernc.org/sqlite"
)

type ConnectionType string

const (
	Sqlite   ConnectionType = "sqlite3"
	Postgres ConnectionType = "postgres"
)

func NewDB(connStr string, connType ConnectionType) *sqlx.DB {
	db, err := apmsql.Open(string(connType), connStr)
	if err != nil {
		panic(fmt.Errorf("db: failed to open connection to database: %w", err))
	}

	sqlxDB := sqlx.NewDb(db, string(connType))
	return sqlxDB
}
