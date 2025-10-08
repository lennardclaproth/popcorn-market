package rest

import "github.com/google/uuid"

type OpenAccountRequest struct {
	UserId  uuid.UUID `json:"uid"`
	Balance float64   `json:"balance"`
}
