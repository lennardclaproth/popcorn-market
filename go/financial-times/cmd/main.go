package main

import (
	"context"
	"log"
	"net/http"
	"time"

	"github.com/popcorn-market/financial-times/api"
	"github.com/popcorn-market/financial-times/config"
	"github.com/popcorn-market/financial-times/internal/application"
	"github.com/popcorn-market/financial-times/internal/infrastructure/mongo"
)

func main() {
	log.Printf("Starting up application")

	// Load configuration
	cfg, err := config.LoadConfig("config/config.yaml")
	if err != nil {
		log.Fatalf("❌ Failed to load config: %v", err)
	}

	// Initialize MongoDB client
	mongoClient, err := mongo.InitMongoClient(cfg.MongoDB.URI)
	if err != nil {
		log.Fatalf("❌ Failed to create MongoDB client: %v", err)
	}
	defer mongo.CloseMongoClient()

	// Initialize the repository
	articleRepo, err := mongo.NewMongoRepository(mongoClient, "popcorn", "articles")

	// Initialize the service
	articleService := service.NewArticleService(articleRepo)

	// Initialize the handler
	articleHandler := api.NewArticleHandler(articleService)

	// Set up the router with the handler
	router := api.SetupRouter(articleHandler)

	// Test MongoDB connection
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()
	if _, err := mongoClient.ListDatabaseNames(ctx, nil); err != nil {
		log.Fatalf("❌ Failed to list databases: %v", err)
	}

	// Start HTTP server on port 8081
	serverAddress := ":8081"
	log.Printf("🚀 Server running on %s", serverAddress)
	if err := http.ListenAndServe(serverAddress, router); err != nil {
		log.Fatalf("❌ Failed to start server: %v", err)
	}
}
