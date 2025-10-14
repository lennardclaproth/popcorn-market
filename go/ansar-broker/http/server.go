package http

import (
	"net/http"

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
func (s *Server) Run() error {
	http.ListenAndServe(s.addr, s.mux)

	return nil
}
