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
		store:     s,
		broker:    b,
		log:       log,
		scheduler: *NewScheduler(ctx, log),
	}
}

func (mgr *Manager) StartScheduler(ctx context.Context) {
	mgr.scheduler.start(ctx)
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

	// if traderCnt is smaller than the set limit 
	// we still need to generate new traders and save them 
	// int the database.
	traderCnt := len(traders)
	if traderCnt < 100 {
		newTraders, err := mgr.generateTraders(100-traderCnt, traderCnt)

		if err != nil {
			return errorx.Trace(fmt.Errorf("manager: failed to generate traders: %w", err))
		}

		traders = append(traders, newTraders...)
	}

	// here we load the traders, if the trader does not
	// have a user id yet it means that they have not been
	// onboarded yet.
	for _, trader := range traders {
		trader.Load(&mgr.broker)
		if trader.UserID == uuid.Nil {
			mgr.onBoardTrader(ctx, trader)
		}
		mgr.startSession(ctx, trader)
	}

	return nil
}

func (mgr *Manager) startSession(ctx context.Context, t trader.Trader){
	session := NewSession(&t, mgr.log)
	mgr.scheduler.AddSession(ctx, t.ID, session)
}

func (mgr *Manager) generateTraders(cnt, traderI int) ([]trader.Trader, error) {
	types := []trader.TraderType{
		trader.TraderTypeScalper,
		trader.TraderTypeSwing,
		trader.TraderTypePosition,
		trader.TraderTypeFundamental,
	}
	traders := make([]trader.Trader, cnt)
	for i := 0; i < cnt; i++ {
		id := fmt.Sprintf("T-%04d", traderI)
		name := fmt.Sprintf("Trader %d", i)

		config := trader.TraderConfig{
			TraderType: types[rand.Intn(len(types))],
			BuyBias:    rand.Float64(), // 0.0 - 1.0
			SellBias:   rand.Float64(),
		}

		trader := trader.NewTrader(id, name, config)
		err := mgr.store.Create(*trader)
		if err != nil {
			return nil, errorx.Trace(fmt.Errorf("loadTraders: failed to execute: %w", err))
		}
		traders = append(traders, *trader)
		traderI += 1
	}

	return traders, nil
}

func (mgr *Manager) onBoardTrader(ctx context.Context, t trader.Trader) error {
	id, err := t.OnBoard(ctx)
	if err != nil {
		return errorx.Trace(fmt.Errorf("manager: failed to onboard trader: %w", err))
	}
	err = mgr.store.UpdateUserID(ctx, t.ID, id)
	if err != nil {
		return errorx.Trace(fmt.Errorf("manager: failed to update trader id: %w", err))
	}
	return nil
}
