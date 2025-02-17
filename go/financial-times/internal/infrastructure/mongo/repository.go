package mongo

import (
	"context"

	"go.mongodb.org/mongo-driver/v2/bson"
	"go.mongodb.org/mongo-driver/v2/mongo"

	"github.com/popcorn-market/financial-times/internal/domain/article"
)

// MongoRepository implements ArticleRepository for MongoDB
type MongoRepository struct {
	client     *mongo.Client
	collection *mongo.Collection
}

// NewMongoRepository initializes a MongoDB repository, returns the pointer to a MongoRepository
func NewMongoRepository(c *mongo.Client, dbName, collectionName string) (*MongoRepository, error) {
	collection := c.Database(string(dbName)).Collection(string(collectionName))

	// The & sign here is used to to take the address of a variable, creating a pointer to that variable.
	return &MongoRepository{client: c, collection: collection}, nil
}

// Close closes the MongoDB connection
func (r *MongoRepository) Close() error {
	return r.client.Disconnect(context.TODO())
}

// Finds articles by their ticker
func (r *MongoRepository) ByTicker(ctx context.Context, t string) ([]*article.CompanyArticle, error) {
	filter := bson.D{{
		Key:   "ticker",
		Value: t,
	}, {
		Key:   "type",
		Value: string(article.Company),
	}}

	cursor, err := r.collection.Find(ctx, filter)
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	var results []*article.CompanyArticle

	for cursor.Next(ctx) {
		var article article.CompanyArticle
		if err := cursor.Decode(&article); err != nil {
			return nil, err
		}
		results = append(results, &article)
	}

	if err := cursor.Err(); err != nil {
		return nil, err
	}

	return results, nil
}

func (r *MongoRepository) Create(ctx context.Context, a article.Article) error {
	doc, err := bson.Marshal(a)
	if err != nil {
		return err
	}

	_, err = r.collection.InsertOne(ctx, doc)
	if err != nil {
		return err
	}

	return nil
}
