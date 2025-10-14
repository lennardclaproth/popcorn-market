package http

func (s *Server) registerRoutes() {
	withRequestLogging := withRequestLogging(s.log)

	s.mux.Handle("/accounts/open", withRequestLogging(handleOpenAccount(*s.app.Accounts, s.log)))
}
