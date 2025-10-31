package db

import (
	"context"
	"fmt"

	"github.com/lennardclaproth/profitron/config"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

func NewDB(ctx context.Context, cfg config.MongoConfig) *mongo.Database {
	dialCtx, cancel := context.WithTimeout(ctx, cfg.ConnTimeout)
	defer cancel()

	opts := options.Client().
		ApplyURI(cfg.URI).
		SetMinPoolSize(cfg.MinPool).
		SetMaxPoolSize(cfg.MaxPool).
		SetAppName(cfg.AppName)

	m, err := mongo.Connect(dialCtx, opts)
	if err != nil {
		panic(fmt.Errorf("db: failed to connect to mongodb: %w", err))
	}

	if err := m.Ping(dialCtx, nil); err != nil {
		_ = m.Disconnect(context.Background())
		panic(fmt.Errorf("db: failed to ping database: %w", err))
	}
	return m.Database(cfg.DBName)
}

func Close(ctx context.Context, db *mongo.Database) {
	db.Client().Disconnect(ctx)
}
