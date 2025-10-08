# Ansar Broker

## OpenApi spec setup

Make sure *swaggo* is installed, you can do this by running the following commands:

```ps
go get -u github.com/swaggo/swag
go install github.com/swaggo/swag/cmd/swag@latest
```

To setup the openapi specification run the following command:

```ps
swag init -g cmd/broker/main.go -o internal/docs
```

This will generate the openapi swagger documentation and store it in the internal/docs folder. Make sure you mount this folder in some way so that your api can serve it.

## Migrations



## Resources
