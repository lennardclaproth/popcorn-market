package usecase

import (
	"context"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/internal/modules/account"
	"github.com/lennardclaproth/ansar-broker/internal/shared"
)

type OpenAccountCommand struct {
	UserId  uuid.UUID
	Balance float64
}

type OpenAccount struct {
	Repository account.Repository
	Logger     shared.Logger
}

func NewOpenAccount(repo account.Repository, log shared.Logger) *OpenAccount {
	return &OpenAccount{
		Repository: repo,
		Logger:     log,
	}
}

func (uc *OpenAccount) Execute(ctx context.Context, command OpenAccountCommand) error {
	a := account.NewAccount(command.UserId, command.Balance)
	err := uc.Repository.Create(a)
	return err
}
