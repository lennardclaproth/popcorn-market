package engine

import (
	"fmt"
	"log/slog"
	"time"

	"github.com/lennardclaproth/profitron/internal/domain"
)

type SessionState string

const (
	SessionStateIdle    SessionState = "idle"
	SessionStateRunning SessionState = "running"
	SessionStatePaused  SessionState = "paused"
	SessionStateStopped SessionState = "stopped"
	SessionStateError   SessionState = "error"
)

// Session holds the trader and the the services with which to fetch the data.
type Session struct {
	trader   *domain.Trader
	logger   *slog.Logger
	tick     time.Duration
	state    SessionState
	message  string
	// Services would be defined here
}

// Creates a new session for the given trader with the given services.
func NewSession(trader *domain.Trader, logger *slog.Logger) *Session {
	return &Session{
		trader: trader,
		logger: logger,
		tick:   100 * time.Millisecond,
		state: SessionStateIdle,
	}
}

// Start begins the session's operations.
func (s *Session) Start() {
	s.state = SessionStateRunning
	s.logger.Info("Session started", "trader_id", s.trader.ID, "tick", s.tick)
	go s.runSessionLoop()
}

// Stop ends the session's operations.
func (s *Session) Stop() {
	s.state = SessionStateStopped
	s.logger.Info("Session stopped", "trader_id", s.trader.ID)
}

func (s *Session) runSessionLoop() {
	for s.state == SessionStateRunning {
		// Here would be the logic to fetch data, analyze, and trade
		s.logger.Debug("Session tick", "trader_id", s.trader.ID)
		time.Sleep(s.tick)
	}
}

func (s *Session) GetStatus() (SessionState, error) {
	if s.state == SessionStateError {
		return "", fmt.Errorf("session in error state: %s", s.message)
	}
	return s.state, nil
}
