package application

import (
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/config"
	"github.com/lennardclaproth/ansar-broker/db"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/internal/babylon"
	"github.com/lennardclaproth/ansar-broker/internal/order"
	"github.com/lennardclaproth/ansar-broker/internal/security"
	"github.com/lennardclaproth/ansar-broker/internal/user"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type App struct {
	Accounts   *account.Service
	Users      *user.Service
	Orders     *order.Service
	Securities *security.Service
	Config     *config.Configuration
}

func NewApp(dbConn *sqlx.DB, log logging.Logger, cfg *config.Configuration) *App {
	userStore := db.NewUserStoreSqlx(dbConn)
	accountStore := db.NewAccountStoreSqlx(dbConn)
	orderStore := db.NewOrderStoreSqlx(dbConn)

	userService := user.NewService(userStore, log)
	accountService := account.NewService(accountStore, log, userService)
	exchangeService := babylon.NewService(cfg)
	securityService := security.NewService(exchangeService, log)

	orderService := order.NewService(orderStore, log, accountService, exchangeService)

	app := &App{
		Accounts:   accountService,
		Users:      userService,
		Orders:     orderService,
		Securities: securityService,
	}

	return app
}
