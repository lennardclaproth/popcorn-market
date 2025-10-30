package db

import (
	"context"
	"time"

	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

// Config is now declared here for simplicity sake, should move to YAML configuration.
type Config struct {
	URI         string        // e.g. "mongodb://localhost:27017"
	DBName      string        // e.g. "popcorn"
	AppName     string        // e.g. "financial-atlas"
	MinPool     uint64        // e.g. 5
	MaxPool     uint64        // e.g. 50
	ConnTimeout time.Duration // e.g. 10 * time.Second
}

func NewDB(ctx context.Context, cfg Config) (*mongo.Database, error) {
	dialCtx, cancel := context.WithTimeout(ctx, cfg.ConnTimeout)
	defer cancel()

	opts := options.Client().
		ApplyURI(cfg.URI).
		SetMinPoolSize(cfg.MinPool).
		SetMaxPoolSize(cfg.MaxPool).
		SetAppName(cfg.AppName)

	m, err := mongo.Connect(dialCtx, opts)
	if err != nil {
		return nil, err
	}

	if err := m.Ping(dialCtx, nil); err != nil {
		_ = m.Disconnect(context.Background())
		return nil, err
	}
	return m.Database(cfg.DBName), nil
}

func Close(ctx context.Context, db *mongo.Database) {
	db.Client().Disconnect(ctx)
}
