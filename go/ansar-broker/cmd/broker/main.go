package main

import (
	"context"
	"fmt"
	"log"
	"log/slog"
	"net/http"

	"github.com/go-chi/chi"
	_ "github.com/lennardclaproth/ansar-broker/internal/docs"
	"github.com/lennardclaproth/ansar-broker/internal/infrastructure"
	module "github.com/lennardclaproth/ansar-broker/internal/modules"
	accountPersistence "github.com/lennardclaproth/ansar-broker/internal/modules/account/persistence"
	"github.com/lennardclaproth/ansar-broker/internal/modules/user"
	userPersistence "github.com/lennardclaproth/ansar-broker/internal/modules/user/persistence"
	"github.com/lennardclaproth/ansar-broker/internal/shared"
	httpSwagger "github.com/swaggo/http-swagger"
)

func main() {
	db := infrastructure.NewDB("")
	logger := infrastructure.NewSlogLogger(slog.LevelDebug)
	validate := shared.NewValidator()
	accountRepo := accountPersistence.NewAccountRepositorySqlx(db)
	userRepo := userPersistence.NewUserRepositorySqlx(db)
	logger.Info(context.Background(), "Loading modules...")
	accountModule := module.NewAccountModule(accountRepo, logger, validate)
	userModule := user.NewUserModule(logger, userRepo, validate)

	logger.Info(context.Background(), "Setting up router...")
	r := chi.NewRouter()

	logger.Info(context.Background(), "Registering middlewares...")
	r.Use(shared.LoggingMiddleware(logger))

	logger.Info(context.Background(), "Making swagger available...")
	r.Get("/swagger/*", httpSwagger.WrapHandler)

	logger.Info(context.Background(), "Mounting routes...")
	r.Mount("/accounts", accountModule.Handler.Routes())
	r.Mount("/users", userModule.Handler.Routes())

	addr := ":6060"
	logger.Info(context.Background(),
		"Ansar Broker is listening for incoming requests...",
		"addr", addr,
		"swagger_url", fmt.Sprintf("http://localhost%s/swagger/index.html", addr),
	)

	if err := http.ListenAndServe(":6060", r); err != nil {
		log.Fatal(err)
	}
}
