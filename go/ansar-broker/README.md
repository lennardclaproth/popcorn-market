# Ansar Broker

## OpenApi spec setup

Make sure *swaggo* is installed, you can do this by running the following commands:

```ps
go get -u github.com/swaggo/swag
go install github.com/swaggo/swag/cmd/swag@latest
```

To setup the openapi specification run the following command:

```ps
swag init -g cmd/api/main.go -o internal/docs
```

This will generate the openapi swagger documentation and store it in the internal/docs folder. Make sure you mount this folder in some way so that your api can serve it.

## Migrations

To run the migrations script and create a sqlite database in the correct folder (make sure you're in the right folder) run the following command:

```ps
goose sqlite3 ./db/ansar-broker.db -dir ./db/migrations/sqlite up 
```

## Config

Make sure you have a YAML config. The developers config should look like this:

```yaml

```

## Resources
