package account

import "github.com/lennardclaproth/ansar-broker/logging"

type AccountService struct {
	Repository Repository
	Logger     logging.Logger
}

func NewAccountService(repo Repository, log logging.Logger) *AccountService {
	return &AccountService{
		Repository: repo,
		Logger:     log,
	}
}

func (s *AccountService) Open(command OpenAccountCommand) error {
	a := NewAccount(command.UserId, command.Balance)
	err := s.Repository.Create(a)
	return err
}
