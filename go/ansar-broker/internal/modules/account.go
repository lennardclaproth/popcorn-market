package modules

import (
	"github.com/go-playground/validator/v10"
	"github.com/lennardclaproth/ansar-broker/internal/modules/account"
	"github.com/lennardclaproth/ansar-broker/internal/modules/account/rest"
	"github.com/lennardclaproth/ansar-broker/internal/modules/account/usecase"
	"github.com/lennardclaproth/ansar-broker/internal/shared"
)

type AccountModule struct {
	Repository account.Repository
	Handler    *rest.Handler
}

// Define module setup
func NewAccountModule(r account.Repository, l shared.Logger, validate *validator.Validate) *AccountModule {
	createUc := usecase.NewOpenAccount(r, l)
	handler := rest.NewHandler(l, validate, createUc)

	return &AccountModule{
		Repository: r,
		Handler:    handler,
	}
}
