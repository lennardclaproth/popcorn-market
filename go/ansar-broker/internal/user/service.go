package user

import (
	"context"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type userService struct {
	repo Repository
	log  logging.Logger
}

func NewUserService(repo Repository, log logging.Logger) *userService {
	return &userService{repo: repo, log: log}
}

func (s *userService) CreateUser(ctx context.Context, cmd CreateUserCommand) (uuid.UUID, error) {
	u := NewUser(cmd.DateOfBirth, cmd.Email, cmd.FirstName, cmd.LastName, cmd.Street, cmd.City, cmd.State, cmd.ZipCode, cmd.Country, cmd.Password)

	err := s.repo.Create(u)

	if err != nil {
		s.log.Error(ctx, "An error occurred while trying to save the user.", err)
		return uuid.Nil, err
	}

	return u.ID, nil
}

func (s *userService) GetUser(ctx context.Context, id int64) (*UserDTO, error) {
	return &UserDTO{}, nil
}
