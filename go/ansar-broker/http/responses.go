package http

import "github.com/google/uuid"

type CreateUserResponse struct {
	ID string `json:"id" example:"123e4567-e89b-12d3-a456-426614174000"`
}

type OpenAccountResponse struct {
	ID      uuid.UUID `json:"id"`
	Balance float64   `json:"balance"`
}
