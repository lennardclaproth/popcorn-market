package transport

import (
	_ "github.com/lennardclaproth/ansar-broker/internal/docs"
	httpSwagger "github.com/swaggo/http-swagger"
)

func (s *Server) registerRoutes() {
	withRequestLogging := withRequestLogging(s.log)

	s.mux.Handle("POST /api/v1/accounts/open", withRequestLogging(s.handleOpenAccount()))
	s.mux.Handle("POST /api/v1/users/create", withRequestLogging(s.handleCreateUser()))
	s.mux.Handle("GET /api/v1/securities/search", withRequestLogging(s.handleSearchListings()))
	s.mux.Handle("POST /api/v1/orders/place", withRequestLogging(s.handlePlaceOrder()))
	s.mux.Handle("GET /swagger/", httpSwagger.WrapHandler)
	s.mux.Handle("GET /api/v1/accounts/active/", withRequestLogging(s.handleGetActiveAccount()))
}
