package engine

import (
	"context"
	"sync"
	"time"

	"github.com/lennardclaproth/profitron/logging"
)

type Scheduler struct {
	sessions map[string]*Session
	logger   logging.Logger
	isActive bool
	mu       sync.RWMutex
}

// Creates a new scheduler to manage multiple sessions.
func NewScheduler(ctx context.Context, logger logging.Logger) *Scheduler {
	s := &Scheduler{
		sessions: make(map[string]*Session),
		logger:   logger,
	}

	return s
}

// Adds a new session to the scheduler.
func (s *Scheduler) AddSession(ctx context.Context, id string, session *Session) {
	s.mu.Lock()
	defer s.mu.Unlock()
	s.sessions[id] = session
}

// Starts the scheduler's operations.
func (s *Scheduler) start(ctx context.Context) {
	s.logger.Info(ctx, "Scheduler started")
	s.isActive = true
	go s.runSchedulerLoop(ctx)
}

func (s *Scheduler) runSchedulerLoop(ctx context.Context) {
	ticker := time.NewTicker(1 * time.Second)
	defer ticker.Stop()

	for s.isActive {
		<-ticker.C // wait for next tick

		s.logger.Debug(ctx, "Scheduler tick", "session_count", len(s.sessions))

		s.mu.RLock()
		for _, session := range s.sessions {
			state, err := session.GetStatus(ctx)
			if err != nil {
				// Correct error handling
				continue
			}
			if state == SessionStateIdle {
				session.Start(ctx)
			}
		}
		s.mu.RUnlock()
	}
}

func (s *Scheduler) Stop(ctx context.Context) {
	s.isActive = false
	s.logger.Info(ctx, "Scheduler stopping, stopping all sessions")
	s.mu.Lock()
	defer s.mu.Unlock()
	for id, session := range s.sessions {
		session.Stop(ctx)
		delete(s.sessions, id)
	}
}
