package account

import "github.com/lennardclaproth/ansar-broker/logging"

type Facade struct {
	AccountService *AccountService
}

func NewFacade(repo Repository, log logging.Logger) *Facade {
	facade := &Facade{
		AccountService: NewAccountService(repo, log),
	}

	return facade
}
