package http

import (
	_ "github.com/lennardclaproth/ansar-broker/internal/docs"
	httpSwagger "github.com/swaggo/http-swagger"
)

func (s *Server) registerRoutes() {
	withRequestLogging := withRequestLogging(s.log)

	s.mux.Handle("POST /api/v1/accounts/open", withRequestLogging(handleOpenAccount(*s.app.Accounts, s.log)))
	s.mux.Handle("POST /api/v1/users/create", withRequestLogging(handleCreateUser(*s.app.Users, s.log)))
	s.mux.Handle("GET /swagger/", httpSwagger.WrapHandler)
}
