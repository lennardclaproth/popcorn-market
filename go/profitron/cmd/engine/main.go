package main

import (
	"context"
	"log/slog"
	"os"
	"os/signal"
	"syscall"

	"github.com/lennardclaproth/profitron/config"
	"github.com/lennardclaproth/profitron/db"
	"github.com/lennardclaproth/profitron/internal/broker"
	"github.com/lennardclaproth/profitron/internal/engine"
	"github.com/lennardclaproth/profitron/logging"
)

// main is intentionally minimal — it just executes run() and handles its error.
func main() {
	if err := run(); err != nil {
		slog.Error("fatal error", "err", err)
		os.Exit(1)
	}
}

// run contains the program’s actual logic and returns an error if something fails.
func run() error {
	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
	defer stop()

	logger := logging.NewSlogLogger(slog.LevelDebug)
	cfg := config.ReadConfig()

	dbInstance, err := db.NewDB(ctx, db.Config(cfg.Mongo))
	if err != nil {
		return err
	}

	store, err := db.NewTraderStore(dbInstance)
	if err != nil {
		return err
	}

	b := broker.NewBroker(cfg.Broker.URI)
	mgr := engine.NewManager(ctx, store, *b, logger)

	mgr.StartScheduler(ctx)

	if err := mgr.LoadTraders(ctx); err != nil {
		return err
	}

	// Wait for termination signal
	<-ctx.Done()

	mgr.StopScheduler(ctx)
	return nil
}
