package account

import "github.com/google/uuid"

type OpenAccountCommand struct {
	UserId  uuid.UUID
	Balance float64
}
