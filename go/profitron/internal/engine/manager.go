package engine

import (
	"context"
	"fmt"
	"math/rand"

	"github.com/google/uuid"
	"github.com/lennardclaproth/profitron/errorx"
	"github.com/lennardclaproth/profitron/internal/broker"
	"github.com/lennardclaproth/profitron/internal/trader"
	"github.com/lennardclaproth/profitron/logging"
)

type TraderStore interface {
	Fetch(ctx context.Context) ([]trader.Trader, error)
	Create(t trader.Trader) error
	UpdateUserID(ctx context.Context, traderID string, userID any) error
}

type Manager struct {
	store     TraderStore
	broker    broker.Broker
	scheduler Scheduler
	log       logging.Logger
}

func NewManager(ctx context.Context, s TraderStore, b broker.Broker, log logging.Logger) *Manager {
	return &Manager{
		store:  s,
		broker: b,
		log:    log,
	}
}

func (mgr *Manager) StartScheduler(ctx context.Context) {
	mgr.scheduler = *NewScheduler(ctx, mgr.log)
}

func (mgr *Manager) StopScheduler(ctx context.Context) {
	mgr.scheduler.Stop(ctx)
}

func (mgr *Manager) LoadTraders(ctx context.Context) error {
	// Fetch from store, if traders.len is 0 we still
	// need to initialize them
	traders, err := mgr.store.Fetch(ctx)
	if err != nil {
		return errorx.Trace(fmt.Errorf("loadTraders: failed to execute: %w", err))
	}

	if len(traders) < 100 {
		types := []trader.TraderType{
			trader.TraderTypeScalper,
			trader.TraderTypeSwing,
			trader.TraderTypePosition,
			trader.TraderTypeFundamental,
		}
		cnt := 100 - len(traders)
		intId := len(traders)
		for i := 0; i < cnt; i++ {
			id := fmt.Sprintf("T-%04d", intId) // -> T-0001 ... T-10000
			name := fmt.Sprintf("Trader %d", i)

			config := trader.TraderConfig{
				TraderType: types[rand.Intn(len(types))],
				BuyBias:    rand.Float64(), // 0.0 - 1.0
				SellBias:   rand.Float64(),
			}

			trader := trader.NewTrader(id, name, config)
			err := mgr.store.Create(*trader)
			if err != nil {
				return errorx.Trace(fmt.Errorf("loadTraders: failed to execute: %w", err))
			}
			traders = append(traders, *trader)
			intId += 1
		}
	}

	for _, trader := range traders {
		trader.Load(&mgr.broker)
		if trader.UserID == uuid.Nil {
			id, err := trader.OnBoard(ctx)
			if err != nil {
				return errorx.Trace(fmt.Errorf("loadTraders: failed to onboard: %w", err))
			}
			err = mgr.store.UpdateUserID(ctx, trader.ID, id)
			if err != nil {
				return errorx.Trace(fmt.Errorf("loadTraders: failed to update id: %w", err))
			}
		}
		session := NewSession(&trader, mgr.log)
		mgr.scheduler.AddSession(ctx, trader.ID, session)
	}

	return nil
}
