package user

import (
	"github.com/go-playground/validator/v10"
	"github.com/lennardclaproth/ansar-broker/internal/modules/user/domain"
	"github.com/lennardclaproth/ansar-broker/internal/modules/user/rest"
	"github.com/lennardclaproth/ansar-broker/internal/modules/user/service"
	"github.com/lennardclaproth/ansar-broker/internal/shared"
)

type UserModule struct {
	Repository domain.Repository
	Handler    *rest.Handler
}

func NewUserModule(l shared.Logger, r domain.Repository, v *validator.Validate) *UserModule {
	ucs := application.UserUseCases{
		UserService: application.NewUserService(r, l),
	}
	handler := rest.NewHandler(l, ucs, v)

	return &UserModule{
		Repository: r,
		Handler:    handler,
	}
}
