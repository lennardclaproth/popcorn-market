package engine

import (
	"context"
	"fmt"
	"time"

	"github.com/lennardclaproth/profitron/internal/trader"
	"github.com/lennardclaproth/profitron/logging"
)

type SessionState int

const (
	SessionStateIdle SessionState = iota
	SessionStateRunning
	SessionStatePaused
	SessionStateStopped
	SessionStateError
)

// Session holds the trader and the the services with which to fetch the data.
type Session struct {
	trader  *trader.Trader
	logger  logging.Logger
	tick    time.Duration
	state   SessionState
	message string
	// Services would be defined here
}

// Creates a new session for the given trader with the given services.
func NewSession(trader *trader.Trader, logger logging.Logger) *Session {
	return &Session{
		trader: trader,
		logger: logger,
		tick:   100 * time.Millisecond,
		state:  SessionStateIdle,
	}
}

// Start begins the session's operations.
func (s *Session) Start(ctx context.Context) {
	s.state = SessionStateRunning
	s.logger.Info(ctx, "Session started", "trader_id", s.trader.ID)
	go s.run(ctx)
}

// Stop ends the session's operations.
func (s *Session) Stop(ctx context.Context) {
	s.state = SessionStateStopped
	s.logger.Info(ctx, "Session stopped", "trader_id", s.trader.ID)
}

func (s *Session) run(ctx context.Context) {
	ticker := time.NewTicker(s.tick)

	for {
		select {
		case <-ctx.Done():
			return
		case <-ticker.C:
			s.logger.Debug(ctx, "Session tick", "trader_id", s.trader.ID)
			s.trader.Trade()
		}
	}
}

func (s *Session) GetStatus(ctx context.Context) (SessionState, error) {
	if s.state == SessionStateError {
		return 0, fmt.Errorf("session in error state: %s", s.message)
	}
	return s.state, nil
}
