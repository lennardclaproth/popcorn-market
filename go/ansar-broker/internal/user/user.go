package user

import (
	"context"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type User struct {
	ID          uuid.UUID
	Email       string
	FirstName   string
	LastName    string
	DateOfBirth time.Time
	Address     Address
	Password    string
	CreatedAt   time.Time
	Accounts    []uuid.UUID
}

type Address struct {
	Street  string
	City    string
	State   string
	ZipCode string
	Country string
}

type Store interface {
	GetByID(id uuid.UUID) (*User, error)
	Create(account *User) error
	Update(account *User) error
	Delete(id uuid.UUID) error
}

type Service struct {
	store Store
	log   logging.Logger
}

func NewService(store Store, log logging.Logger) *Service {
	return &Service{store: store, log: log}
}

type CreateUserCommand struct {
	DateOfBirth time.Time
	Email,
	FirstName,
	LastName,
	Street,
	City,
	State,
	ZipCode,
	Country,
	Password string
}

func (s *Service) CreateUser(ctx context.Context, cmd CreateUserCommand) (uuid.UUID, error) {
	u := &User{
		ID:          uuid.New(),
		Email:       cmd.Email,
		FirstName:   cmd.FirstName,
		LastName:    cmd.LastName,
		DateOfBirth: cmd.DateOfBirth,
		Address: Address{
			Street:  cmd.Street,
			City:    cmd.City,
			State:   cmd.State,
			ZipCode: cmd.ZipCode,
			Country: cmd.Country,
		},
		Password:  cmd.Password,
		CreatedAt: time.Now(),
		Accounts:  []uuid.UUID{},
	}

	err := s.store.Create(u)

	if err != nil {
		return uuid.Nil, err
	}

	return u.ID, nil
}

// CanOpenAccount checks if the user that is passed by the user id is able to open
// an account.
func (s *Service) CanOpenAccount(ctx context.Context, uid uuid.UUID) (bool, error) {
	return true, nil
}
