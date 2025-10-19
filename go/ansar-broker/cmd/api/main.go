package main

import (
	"context"
	"fmt"
	"log/slog"
	"os"
	"os/signal"

	"github.com/lennardclaproth/ansar-broker/config"
	"github.com/lennardclaproth/ansar-broker/db"
	"github.com/lennardclaproth/ansar-broker/http"
	"github.com/lennardclaproth/ansar-broker/internal/application"
	"github.com/lennardclaproth/ansar-broker/logging"
)

func run(ctx context.Context, args []string) error {
	ctx, cancel := signal.NotifyContext(ctx, os.Interrupt)
	defer cancel()

	cfg := config.ReadConfig()

	// db := db.NewDB("file:db/ansar-broker.db?_foreign_keys=on", db.Sqlite)
	db := db.NewDB(cfg.Database.ConnStr, db.ConnectionType(cfg.Database.Type))
	// log := logging.NewSlogLogger(slog.LevelInfo)
	log := logging.NewSlogLogger(slog.LevelInfo)
	app := application.NewApp(db, log, cfg)

	server := http.NewServer(":6060", app, log)

	if err := server.Run(ctx); err != nil {
		return fmt.Errorf("server error: %w", err)
	}

	return nil
}

func main() {
	ctx := context.Background()
	if err := run(ctx, os.Args); err != nil {
		fmt.Fprintf(os.Stderr, "%s\n", err)
		os.Exit(1)
	}
}
