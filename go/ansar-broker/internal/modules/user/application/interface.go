package application

import (
	"context"

	"github.com/google/uuid"
)

// Public interface for interacting with the user domain
type Service interface {
	CreateUser(ctx context.Context, cmd CreateUserCommand) (uuid.UUID, error)
	GetUser(ctx context.Context, id int64) (*UserDTO, error)
}

