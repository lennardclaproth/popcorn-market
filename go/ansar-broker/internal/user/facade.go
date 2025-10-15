package user

import "github.com/lennardclaproth/ansar-broker/logging"

type Facade struct {
	UserService *userService
}

func NewFacade(repo Repository, log logging.Logger) *Facade {
	userFacade := &Facade{
		UserService: NewUserService(repo, log),
	}

	return userFacade
}
