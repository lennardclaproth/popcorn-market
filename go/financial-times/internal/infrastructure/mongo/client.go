package mongo

import (
	"context"
	"fmt"
	"log"
	"time"

	"go.mongodb.org/mongo-driver/v2/bson"
	"go.mongodb.org/mongo-driver/v2/mongo"
	"go.mongodb.org/mongo-driver/v2/mongo/options"
)

var mongoClient *mongo.Client

// InitMongoClient initializes a single MongoDB client (Singleton)
func InitMongoClient(uri string) (*mongo.Client, error) {
	if mongoClient != nil {
		return mongoClient, nil
	}

	client, err := mongo.Connect(options.Client().ApplyURI(uri))

	if err != nil {
		return nil, fmt.Errorf("❌ MongoDB connection failed retrying in %v sec. inner error: %v", 5, err)
	}

	var result bson.M
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	if err := client.Database("admin").RunCommand(ctx, bson.D{bson.E{Key: "ping", Value: 1}}).Decode(&result); err != nil {
		log.Fatalf("❌ MongoDB ping failed: %v", err)
	}
	log.Println("✅ MongoDB connection verified successfully!")

	log.Println("✅ MongoDB connected successfully")
	mongoClient = client
	return mongoClient, nil
}

// CloseMongoClient closes the MongoDB connection
func CloseMongoClient() {
	if mongoClient != nil {
		if err := mongoClient.Disconnect(context.TODO()); err != nil {
			log.Printf("Failed to disconnect MongoDB: %v", err)
		} else {
			log.Println("✅ MongoDB connection closed")
		}
	}
}
