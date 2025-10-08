package rest

import "github.com/google/uuid"

type OpenAccountResponse struct {
	ID      uuid.UUID `json:"id"`
	Balance float64   `json:"balance"`
}
