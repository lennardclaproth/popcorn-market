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
goose sqlite3 ./ansar-broker.db -dir ./migrations/sqlite up 
```

## Config

Make sure you have a YAML config. The developers config should look like this:

```yaml
server:
  port: 6060

database:
  connection_string: file:ansar-broker.db?_foreign_keys=on
  type: sqlite3

babylon_exchange:
  uri: https://localhost:7199

apm:
  server_url: http://localhost:8200
  service_name: ansar-broker
  environment: development
  secret_token: 
  verify_server_cert: false
  log_level: warning

```

## Features

### Market-Data

In the market data domain we need to provide real time market data. To realize this we need to use a few somewhat more complicated patterns. First of all we use web sockets so that a client can connect via a websocket and recieve data via a push from the server. When a trade executed event comes in and the ticker is stored in the securities cache we need to invalidate that cache entry. Every so many seconds a background task runs that requests the most up to date data from the exchange for the securities that have invalidated caches. We also need a hub to which a client subscribes and can subscribe to a security. Here we maintain a map from security to client and from client to security. It is important that the worker publishes to a channel so that the websocket server can update it's clients.

## Resources
