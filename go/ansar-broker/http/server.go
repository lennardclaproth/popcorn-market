package http

import (
	"context"
	"fmt"
	"net/http"
	"time"

	"github.com/lennardclaproth/ansar-broker/internal/application"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type Server struct {
	addr string
	app  *application.App
	log  logging.Logger
	mux  *http.ServeMux
}

// NewServer creates and returns a new Server for the given address and database.
func NewServer(addr string, app *application.App, log logging.Logger) *Server {
	s := &Server{
		addr: addr,
		app:  app,
		log:  log,
		mux:  http.NewServeMux(),
	}

	s.registerRoutes()
	return s
}

// Run starts the http server on the address provided
func (s *Server) Run(ctx context.Context) error {
	server := &http.Server{
		Addr:    s.addr,
		Handler: s.mux,
	}

	s.log.Info(context.Background(),
		"Ansar Broker is listening for incoming requests...",
		"addr", s.addr,
		"swagger_url", fmt.Sprintf("http://localhost%s/swagger/index.html", s.addr),
	)
	// Run server in a goroutine
	errChan := make(chan error, 1)
	go func() {
		errChan <- server.ListenAndServe()
	}()

	// Wait for interrupt or server error
	select {
	case <-ctx.Done():
		s.log.Info(ctx, "Shutting down gracefully...")
		shutdownCtx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
		defer cancel()
		if err := server.Shutdown(shutdownCtx); err != nil {
			s.log.Error(ctx, "graceful shutdown failed", err)
			return err
		}
		s.log.Info(ctx, "Server stopped cleanly.")
		return nil
	case err := <-errChan:
		if err != nil && err != http.ErrServerClosed {
			s.log.Error(ctx, "server failed", err)
			return err
		}
		return nil
	}
}
