package http

import (
	"net/http"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/logging"
)

func handleOpenAccount(accounts account.Facade, log logging.Logger) http.HandlerFunc {
	type request struct {
		UserId  uuid.UUID `json:"uid"`
		Balance float64   `json:"balance"`
	}
	type response struct {
		ID      uuid.UUID `json:"id"`
		Balance float64   `json:"balance"`
	}

	return func(w http.ResponseWriter, r *http.Request) {
		// Define handler body
	}
}
