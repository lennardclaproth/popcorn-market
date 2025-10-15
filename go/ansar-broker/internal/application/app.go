package application

import (
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/db"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/internal/user"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type App struct {
	Accounts *account.Facade
	Users    *user.Facade
}

func NewApp(dbConn *sqlx.DB, log logging.Logger) *App {
	ur := db.NewUserRepositorySqlx(dbConn)
	ar := db.NewAccountRepositorySqlx(dbConn)

	af := account.NewFacade(ar, log)
	uf := user.NewFacade(ur, log)
	app := &App{
		Accounts: af,
		Users: uf,
	}

	return app
}
