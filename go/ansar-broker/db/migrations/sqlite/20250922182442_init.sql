-- +goose Up

CREATE TABLE users (
    id TEXT PRIMARY KEY, -- UUID stored as text
    email TEXT NOT NULL UNIQUE,
    firstname TEXT NOT NULL,
    lastname TEXT NOT NULL,
    date_of_birth DATE NOT NULL,
    password TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
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
    id TEXT PRIMARY KEY, -- UUID stored as text
    user_id TEXT NOT NULL,
    account_number TEXT NOT NULL,
    balance REAL NOT NULL DEFAULT 0,
    status NUMBER NOT NULL,
    total_value REAL NOT NULL DEFAULT 0,
    unrealized_pnl REAL NOT NULL DEFAULT 0,
    realized_pnl REAL NOT NULL DEFAULT 0,
    opened_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at DATETIME,
    is_active BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE holdings (
    symbol TEXT NOT NULL,
    account_id TEXT NOT NULL,
    quantity REAL NOT NULL DEFAULT 0,
    avg_price REAL NOT NULL DEFAULT 0,
    PRIMARY KEY (account_id, symbol),
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE CASCADE,
    FOREIGN KEY (symbol) REFERENCES securities(symbol) ON DELETE NO ACTION
);

CREATE TABLE orders (
    id TEXT PRIMARY KEY,
    order_id TEXT DEFAULT NULL,
    account_id TEXT NOT NULL,
    ticker TEXT NOT NULL,
    quantity NUMBER NOT NULL DEFAULT 0,
    price REAL NOT NULL DEFAULT 0,
    order_side NUMBER NOT NULL,
    order_type NUMBER NOT NULL,
    order_status NUMBER NOT NULL,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE CASCADE,
    FOREIGN KEY (ticker) REFERENCES securities(symbol) ON DELETE NO ACTION
);

-- +goose Down
DROP TABLE IF EXISTS holdings;
DROP TABLE IF EXISTS securities;
DROP TABLE IF EXISTS accounts;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS orders
