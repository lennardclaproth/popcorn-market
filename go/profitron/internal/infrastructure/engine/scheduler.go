package engine

import (
	"log/slog"
	"sync"
	"time"
)

type Scheduler struct {
	sessions map[string]*Session
	logger   *slog.Logger
	isActive bool
	mu       sync.RWMutex
}

// Creates a new scheduler to manage multiple sessions.
func NewScheduler(logger *slog.Logger) *Scheduler {
	s := &Scheduler{
		sessions: make(map[string]*Session),
		logger:   logger,
	}

	s.start()

	return s
}

// Adds a new session to the scheduler.
func (s *Scheduler) AddSession(id string, session *Session) {
	s.mu.Lock()
	defer s.mu.Unlock()
	s.sessions[id] = session
}

// Starts the scheduler's operations.
func (s *Scheduler) start() {
	s.logger.Info("Scheduler started")
	s.isActive = true
	go s.runSchedulerLoop()
}

func (s *Scheduler) runSchedulerLoop() {
	ticker := time.NewTicker(1 * time.Second)
	defer ticker.Stop()

	for s.isActive {
		<-ticker.C // wait for next tick

		s.logger.Debug("Scheduler tick", "session_count", len(s.sessions))

		s.mu.RLock()
		for _, session := range s.sessions {
			state, err := session.GetStatus()
			if err != nil {
				s.logger.Error("Error getting session status",
					"session_id", session.trader.ID,
					"error", err)
				continue
			}
			if state == SessionStateIdle {
				session.Start()
			}
		}
		s.mu.RUnlock()
	}
}

func (s *Scheduler) Stop() {
	s.isActive = false
	s.logger.Info("Scheduler stopping, stopping all sessions")
	s.mu.Lock()
	defer s.mu.Unlock()
	for id, session := range s.sessions {
		session.Stop()
		delete(s.sessions, id)
	}
}
