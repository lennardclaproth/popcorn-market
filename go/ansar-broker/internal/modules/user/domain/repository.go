package domain

import "github.com/google/uuid"

type Repository interface {
	GetByID(id uuid.UUID) (*User, error)
	Create(account *User) error
	Update(account *User) error
	Delete(id uuid.UUID) error
}
