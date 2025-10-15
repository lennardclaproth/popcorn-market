package main

import (
	"context"
	"fmt"
	"log/slog"
	"math/rand"
	"os"
	"os/signal"
	"syscall"

	"github.com/lennardclaproth/profitron/internal/domain"
	"github.com/lennardclaproth/profitron/internal/infrastructure/engine"
)

func main() {
	logger := slog.New(slog.NewTextHandler(os.Stdout, &slog.HandlerOptions{
		Level: slog.LevelDebug,
	}))

	scheduler := engine.NewScheduler(logger)

	var traders []*domain.Trader
	types := []domain.TraderType{
		domain.TraderTypeScalper,
		domain.TraderTypeSwing,
		domain.TraderTypePosition,
		domain.TraderTypeFundamental,
	}

	// Create traders
	for i := 1; i <= 10000; i++ {
		id := fmt.Sprintf("T-%04d", i) // -> T-0001 ... T-10000
		name := fmt.Sprintf("Trader %d", i)

		config := domain.TraderConfig{
			TraderType: types[rand.Intn(len(types))],
			BuyBias:    rand.Float64(), // 0.0 - 1.0
			SellBias:   rand.Float64(),
		}

		trader := domain.NewTrader(id, name, config)

		traders = append(traders, trader)
	}

	// Create and add sessions for each trader
	for _, trader := range traders {
		session := engine.NewSession(trader, logger)
		scheduler.AddSession(trader.ID, session)
	}

	ctx, stop := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)
	defer stop()

	<-ctx.Done() // wait until Ctrl+C
	scheduler.Stop()
}
