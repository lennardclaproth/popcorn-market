package engine

import (
	"context"
	"sync"
	"sync/atomic"
	"time"

	"github.com/lennardclaproth/profitron/logging"
)

type Scheduler struct {
	sessions map[string]*Session
	logger   logging.Logger
	isActive atomic.Bool
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
	s.logger.Info(ctx, "Starting scheduler")
	// atomically set store as active
	s.isActive.Store(true)
	// run the run function in a seperate go routine.
	go s.run(ctx)
	s.logger.Info(ctx, "Scheduler started")
}

func (s *Scheduler) run(ctx context.Context) {
	// instantiate a ticker
	ticker := time.NewTicker(1 * time.Second)
	defer ticker.Stop()
	// while the scheduler is active we check the available
	// sessions and run the sessions if needed
	for s.isActive.Load() {
		select {
		case <-ctx.Done():
			s.logger.Info(ctx, "scheduler stopped via context")
			return
		// on every thick we loop over the available sessions
		// and check its status. If a session is idle we start it
		case <-ticker.C:
			s.logger.Debug(ctx, "Scheduler tick", "session_count", len(s.sessions))
			// we use a read lock here because via the add
			// session function we can add new sessions to the
			// session list.
			s.mu.RLock()
			for _, session := range s.sessions {
				state, err := session.GetStatus(ctx)
				if err == nil && state == SessionStateIdle {
					session.Start(ctx)
				}
			}
			s.mu.RUnlock()
		}
	}
}

func (s *Scheduler) Stop(ctx context.Context) {
	s.isActive.Store(false)
	s.logger.Info(ctx, "Scheduler stopping, stopping all sessions")
	s.mu.Lock()
	defer s.mu.Unlock()
	for id, session := range s.sessions {
		session.Stop(ctx)
		delete(s.sessions, id)
	}
}
