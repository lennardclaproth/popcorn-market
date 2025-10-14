package db

import (
	"log"

	"github.com/jmoiron/sqlx"
	// _ "github.com/lib/pq"
	_ "modernc.org/sqlite"
)

type ConnectionType string

const (
	Sqlite  ConnectionType = "sqlite"
	Postgre ConnectionType = "postgres"
)

func NewDB(connStr string, connType ConnectionType) *sqlx.DB {
	db, err := sqlx.Open(string(connType), connStr)
	if err != nil {
		log.Fatalf("failed to open db: %v", err)
	}

	return db
}
