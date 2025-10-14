package account

import "github.com/google/uuid"

type Repository interface {
	GetByID(id uuid.UUID) (*Account, error)
	GetByUserID(userID uuid.UUID) (*Account, error)
	Create(account *Account) error
	Update(account *Account) error
	Delete(id uuid.UUID) error
}
