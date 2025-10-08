-- +goose Up

CREATE TABLE users (
    id UUID PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    firstname TEXT NOT NULL,
    lastname TEXT NOT NULL,
    date_of_birth DATE NOT NULL,
    password TEXT NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    street TEXT,
    city TEXT,
    state TEXT,
    zip_code TEXT,
    country TEXT
);

CREATE TABLE securities (
    symbol TEXT PRIMARY KEY,
    name TEXT NOT NULL
);

CREATE TABLE accounts (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    account_number TEXT NOT NULL,
    balance DOUBLE PRECISION NOT NULL DEFAULT 0,
    status TEXT NOT NULL,
    total_value DOUBLE PRECISION NOT NULL DEFAULT 0,
    unrealized_pnl DOUBLE PRECISION NOT NULL DEFAULT 0,
    realized_pnl DOUBLE PRECISION NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    deleted_at TIMESTAMPTZ,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE holdings (
    symbol TEXT NOT NULL,
    account_id UUID NOT NULL,
    quantity DOUBLE PRECISION NOT NULL DEFAULT 0,
    avg_price DOUBLE PRECISION NOT NULL DEFAULT 0,
    PRIMARY KEY (account_id, symbol),
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE CASCADE,
    FOREIGN KEY (symbol) REFERENCES securities(symbol) ON DELETE RESTRICT
);

-- +goose Down
DROP TABLE IF EXISTS holdings;
DROP TABLE IF EXISTS securities;
DROP TABLE IF EXISTS accounts;
DROP TABLE IF EXISTS users;
