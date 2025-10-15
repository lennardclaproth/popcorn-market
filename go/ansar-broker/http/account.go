package http

import (
	"context"
	"net/http"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type openAccountRequest struct {
	UserId  uuid.UUID `json:"uid"`
	Balance float64   `json:"balance"`
}

type openAccountResponse struct {
	ID      uuid.UUID `json:"id"`
	Balance float64   `json:"balance"`
}

// Implement the Validator interface for this specific request type
func (r openAccountRequest) Valid(ctx context.Context) map[string]string {
	problems := make(map[string]string)

	if r.UserId == uuid.Nil {
		problems["uid"] = "user ID is required"
	}
	if r.Balance < 0 {
		problems["balance"] = "balance cannot be negative"
	}

	return problems
}

// Create godoc
// @Summary      Create account
// @Description  Creates a new account with initial balance
// @Tags         accounts
// @Accept       json
// @Produce      json
// @Param        input  body  openAccountRequest  true  "Account info"
// @Success      200  {object}  openAccountResponse
// @Router       /api/v1/accounts/open [post]
func handleOpenAccount(accounts account.Facade, log logging.Logger) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		// Define handler body
	}
}
