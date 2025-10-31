package db

import (
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/lennardclaproth/profitron/errorx"
	"github.com/lennardclaproth/profitron/internal/trader"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

type MongoTraderStore struct {
	col            *mongo.Collection
	defaultTimeout time.Duration
}

var (
	ErrDuplicateEmail = errors.New("trader with this email already exists")
)

func NewTraderStore(db *mongo.Database) *MongoTraderStore {
	col := db.Collection("traders")

	if err := ensureTraderIndexes(context.Background(), col); err != nil {
		panic(fmt.Errorf("db: failed to ensure trader indexes exist: %w", err))
	}

	return &MongoTraderStore{
		col:            col,
		defaultTimeout: 5 * time.Second,
	}
}

func ensureTraderIndexes(ctx context.Context, col *mongo.Collection) error {
	cctx, cancel := context.WithTimeout(ctx, 10*time.Second)
	defer cancel()

	models := []mongo.IndexModel{
		{
			Keys: bson.D{{Key: "email", Value: 1}},
			Options: options.Index().
				SetName("u_email").
				SetUnique(true),
		},
		{
			Keys: bson.D{
				{Key: "status", Value: 1},
				{Key: "config.trader_type", Value: 1},
			},
			Options: options.Index().
				SetName("i_status_type"),
		},
	}

	_, err := col.Indexes().CreateMany(cctx, models)
	return err
}

func (s *MongoTraderStore) Create(t trader.Trader) error {
	ctx, cancel := context.WithTimeout(context.Background(), s.defaultTimeout)
	defer cancel()

	_, err := s.col.InsertOne(ctx, t)
	if err != nil {
		return errorx.Trace(fmt.Errorf("db: failed to store new trader: %w", err))
	}
	return nil
}

func (s *MongoTraderStore) Fetch(ctx context.Context) ([]trader.Trader, error) {
	// if a context has a deadline, respect that deadline otherwise spawn
	// a context with a timeout.
	if _, hasDeadline := ctx.Deadline(); !hasDeadline {
		var cancel context.CancelFunc
		ctx, cancel = context.WithTimeout(ctx, s.defaultTimeout)
		defer cancel()
	}

	// gets a mongodb cursor
	cur, err := s.col.Find(ctx, bson.D{}, nil)
	if err != nil {
		return nil, errorx.Trace(fmt.Errorf("db: failed to fetch trader: %w", err))
	}
	defer cur.Close(ctx)
	// loop over the results of the cursoer and decode into a trader.
	var out []trader.Trader
	for cur.Next(ctx) {
		var t trader.Trader
		if err := cur.Decode(&t); err != nil {
			return nil, err
		}
		out = append(out, t)
	}
	return out, cur.Err()
}

func (s *MongoTraderStore) UpdateUserID(ctx context.Context, traderID string, userID any) error {
	filter := bson.M{"trader_id": traderID}
	update := bson.M{"$set": bson.M{"user_id": userID}}
	_, err := s.col.UpdateOne(ctx, filter, update)
	if err != nil {
		return errorx.Trace(fmt.Errorf("db: failed to update user id on a trader: %w", err))
	}
	return nil
}
