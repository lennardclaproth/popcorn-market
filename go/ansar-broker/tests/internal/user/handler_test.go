package rest_test

import (
	"bytes"
	"context"
	"encoding/json"
	"log/slog"
	"net/http"
	"net/http/httptest"
	"testing"
	"time"

	_ "modernc.org/sqlite"

	"github.com/go-chi/chi"
	"github.com/jmoiron/sqlx"
	"github.com/pressly/goose/v3"

	"github.com/lennardclaproth/ansar-broker/db/migrations"
	"github.com/lennardclaproth/ansar-broker/internal/infrastructure"
	module "github.com/lennardclaproth/ansar-broker/internal/modules"
	"github.com/lennardclaproth/ansar-broker/internal/modules/user/persistence"
	"github.com/lennardclaproth/ansar-broker/internal/shared"
)

// minimal logger for testing
type testLogger struct{}

func (t *testLogger) Info(ctx context.Context, msg string, args ...interface{})             {}
func (t *testLogger) Error(ctx context.Context, msg string, err error, args ...interface{}) {}

func migrateSchema(db *sqlx.DB) error {
	goose.SetBaseFS(migrations.SQLite) // use embedded migrations
	if err := goose.SetDialect("sqlite3"); err != nil {
		return err
	}
	return goose.Up(db.DB, "sqlite") // path is now virtual inside embed.FS
}

func setupInMemoryServer(t *testing.T) (*httptest.Server, *sqlx.DB) {
	t.Helper()

	// open in-memory sqlite DB
	db, err := sqlx.Open("sqlite", ":memory:")
	if err != nil {
		t.Fatalf("failed to open sqlite db: %v", err)
	}

	// create schema
	if err := migrateSchema(db); err != nil {
		t.Fatalf("failed to migrate schema: %v", err)
	}

	// build repository using the sqlite db
	repo := persistence.NewUserRepositorySqlx(db) // Your repository that uses *sql.DB
	validate := shared.NewValidator()
	logger := infrastructure.NewSlogLogger(slog.LevelDebug)

	// build module
	userModule := module.NewUserModule(logger, repo, validate)

	// create router
	r := chi.NewRouter()
	r.Use(shared.LoggingMiddleware(logger))
	r.Mount("/users", userModule.Handler.Routes())

	return httptest.NewServer(r), db
}

func TestCreateUser_InMemory(t *testing.T) {
	server, db := setupInMemoryServer(t)
	defer server.Close()
	defer db.Close()

	body := map[string]string{
		"email":       "alice@example.com",
		"firstName":   "Alice",
		"lastName":    "Wonderland",
		"dateOfBirth": time.Now().AddDate(-25, 0, 0).Format(time.DateOnly),
		"street":      "123 Magic Lane",
		"city":        "Fictionville",
		"state":       "TX",
		"zipCode":     "77777",
		"country":     "USA",
		"password":    "supersecure",
	}

	jsonBody, _ := json.Marshal(body)
	resp, err := http.Post(server.URL+"/users", "application/json", bytes.NewBuffer(jsonBody))
	if err != nil {
		t.Fatalf("failed to send request: %v", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		t.Fatalf("expected status 200 OK, got %d", resp.StatusCode)
	}

	// Verify DB contents directly
	var count int
	err = db.QueryRow("SELECT COUNT(*) FROM users WHERE email = ?", "alice@example.com").Scan(&count)
	if err != nil {
		t.Fatalf("failed to query sqlite db: %v", err)
	}
	if count != 1 {
		t.Fatalf("expected 1 user, got %d", count)
	}
}
