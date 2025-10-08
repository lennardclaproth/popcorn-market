package infrastructure

import (
	"log"

	"github.com/jmoiron/sqlx"
	// _ "github.com/lib/pq"
	_ "modernc.org/sqlite"
)

func NewDB(dsn string) *sqlx.DB {
	db, err := sqlx.Open("sqlite", "file:db/ansar-broker.db?_foreign_keys=on") // db, err := sqlx.Open("postgres", dsn)
	if err != nil {
		log.Fatalf("failed to open db: %v", err)
	}

	return db
}
