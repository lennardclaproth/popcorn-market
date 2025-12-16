package app

import (
	"context"
	"fmt"
	"time"

	"github.com/google/uuid"
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/config"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/internal/exchange"
	"github.com/lennardclaproth/ansar-broker/internal/order"
	"github.com/lennardclaproth/ansar-broker/internal/security"
	"github.com/lennardclaproth/ansar-broker/internal/user"
	"github.com/lennardclaproth/ansar-broker/logging"
	"github.com/lennardclaproth/ansar-broker/storage"
)

type App struct {
	accounts   *account.Accounts
	users      *user.Service
	orders     *order.Service
	securities *security.Service
}

func New(dbConn *sqlx.DB, log logging.Logger, cfg *config.Configuration) *App {
	userStore := storage.NewUserStoreSqlx(dbConn)
	accountStore := storage.NewAccountStoreSqlx(dbConn)
	orderStore := storage.NewOrderStoreSqlx(dbConn)

	userService := user.NewService(userStore, log)
	accountService := account.NewService(accountStore, log, userService)
	exchangeService := exchange.NewClient(fmt.Sprintf("https://%s", cfg.Exchange.URI), exchange.WithCircuitBreaker(3, 3*time.Second), exchange.WithGRPC(cfg.Exchange.URI))
	securityService := security.NewService(exchangeService, log)

	orderService := order.NewService(orderStore, log, accountService, exchangeService)

	return &App{
		accounts:   accountService,
		users:      userService,
		orders:     orderService,
		securities: securityService,
	}
}

func (a *App) OpenAccount(ctx context.Context, cmd account.OpenAccountCommand) (string, error) {
	return a.accounts.Open(ctx, cmd)
}
func (a *App) CreateUser(ctx context.Context, cmd user.CreateUserCommand) (uuid.UUID, error) {
	return a.users.CreateUser(ctx, cmd)
}
func (a *App) SearchListings(ctx context.Context, query security.GetListingsQuery) ([]security.Listing, error) {
	return a.securities.SearchListings(ctx, query)
}
func (a *App) PlaceOrder(ctx context.Context, cmd order.PlaceOrderCommand) (string, error) {
	return a.orders.PlaceOrder(ctx, cmd)
}
func (a *App) GetActiveAccount(ctx context.Context, uid uuid.UUID) (account.AccountInfo, error) {
	return a.accounts.GetActiveAccount(ctx, uid)
}
