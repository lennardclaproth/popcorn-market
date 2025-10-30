package db

import (
	"context"
	"errors"
	"time"

	"github.com/lennardclaproth/profitron/internal/trader"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

// MongoTraderStore implements Create + Fetch on top of MongoDB.
type MongoTraderStore struct {
	col            *mongo.Collection
	defaultTimeout time.Duration
}

var (
	// Surface a friendly error when email is already taken
	ErrDuplicateEmail = errors.New("trader with this email already exists")
)

// NewTraderStore wires the collection and ensures indexes.
// You can pass db.Collection("traders") from your bootstrap.
func NewTraderStore(db *mongo.Database) (*MongoTraderStore, error) {
	col := db.Collection("traders")

	// Create indexes once at startup
	if err := ensureTraderIndexes(context.Background(), col); err != nil {
		return nil, err
	}

	return &MongoTraderStore{
		col:            col,
		defaultTimeout: 5 * time.Second, // sane default; override via setter if needed
	}, nil
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

// WithTimeout lets you override the default per-call deadline.
func (s *MongoTraderStore) WithTimeout(d time.Duration) *MongoTraderStore {
	cp := *s
	cp.defaultTimeout = d
	return &cp
}

func (s *MongoTraderStore) Create(t trader.Trader) error {
	ctx, cancel := context.WithTimeout(context.Background(), s.defaultTimeout)
	defer cancel()

	_, err := s.col.InsertOne(ctx, t)
	if err != nil {
		var we mongo.WriteException
		if errors.As(err, &we) {
			for _, e := range we.WriteErrors {
				if e.Code == 11000 {
					return ErrDuplicateEmail
				}
			}
		}
		return err
	}
	return nil
}

// Fetch returns all traders (bounded by reasonable server-side timeout).
// You can add filters/pagination later without changing the interface.
func (s *MongoTraderStore) Fetch(ctx context.Context) ([]trader.Trader, error) {
	// Respect caller context but guard with a soft deadline if none present.
	if _, hasDeadline := ctx.Deadline(); !hasDeadline {
		var cancel context.CancelFunc
		ctx, cancel = context.WithTimeout(ctx, s.defaultTimeout)
		defer cancel()
	}

	cur, err := s.col.Find(ctx, bson.D{}, nil)
	if err != nil {
		return nil, err
	}
	defer cur.Close(ctx)

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
	return err
}
