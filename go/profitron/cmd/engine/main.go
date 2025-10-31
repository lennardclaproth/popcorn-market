package main

import (
	"context"
	"fmt"
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

// run contains the program’s actual logic and returns an error if something fails.
func run(ctx context.Context, args []string) error {
	ctx, stop := signal.NotifyContext(ctx, os.Interrupt, syscall.SIGTERM)
	defer stop()

	// app initialization
	cfg := config.ReadConfig()
	dbInstance := db.NewDB(ctx, cfg.Mongo)
	logger := logging.NewSlogLogger(slog.LevelDebug)
	store := db.NewTraderStore(dbInstance)
	b := broker.NewBroker(cfg.Broker.URI)
	mgr := engine.NewManager(ctx, store, *b, logger)

	// Starts the scheduler
	mgr.StartScheduler(ctx)

	// load the traders
	if err := mgr.LoadTraders(ctx); err != nil {
		return err
	}

	// Wait for termination signal
	<-ctx.Done()

	mgr.StopScheduler(ctx)
	return nil
}

func main() {
	ctx := context.Background()
	if err := run(ctx, os.Args); err != nil {
		slog.Error("fatal error", "err", err)
		fmt.Fprintf(os.Stderr, "%s\n", err)
		os.Exit(1)
	}
}
