package main

import (
	"context"
	"fmt"
	"log/slog"
	"os"
	"os/signal"

	"github.com/lennardclaproth/ansar-broker/config"
	"github.com/lennardclaproth/ansar-broker/internal/app"
	"github.com/lennardclaproth/ansar-broker/logging"
	"github.com/lennardclaproth/ansar-broker/storage"
	"github.com/lennardclaproth/ansar-broker/transport"
)

func run(ctx context.Context, args []string) error {
	ctx, stop := signal.NotifyContext(ctx, os.Interrupt)
	defer stop()

	cfg := config.ReadConfig()

	db := storage.NewDB(cfg.Database.ConnStr, storage.ConnectionType(cfg.Database.Type))

	log := logging.NewSlogLogger(slog.LevelInfo)
	app := app.New(db, log, cfg)

	server := transport.NewServer(fmt.Sprintf(":%d", cfg.Server.Port), app, log)

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
