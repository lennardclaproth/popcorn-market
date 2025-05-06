CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'OrderBook') THEN
            CREATE SCHEMA "OrderBook";
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE TABLE "OrderBook"."Company" (
        "Id" UUID NOT NULL,
        "Ticker" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "StockPriceUSD" NUMERIC(18,6) NOT NULL,
        CONSTRAINT "PK_Company" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE TABLE "OrderBook"."OrderBook" (
        "Id" UUID NOT NULL,
        "Ticker" TEXT NOT NULL,
        CONSTRAINT "PK_OrderBook" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE TABLE "OrderBook"."BuyOrder" (
        "Id" UUID NOT NULL,
        "OrderType" INTEGER NOT NULL,
        "OrderBookId" UUID NOT NULL,
        "StockSymbol" TEXT NOT NULL,
        "TraderId" TEXT NOT NULL,
        "Price" NUMERIC(18,6) NOT NULL,
        "Quantity" INTEGER NOT NULL,
        "Timestamp" TIMESTAMPTZ NOT NULL,
        "Status" INTEGER NOT NULL,
        CONSTRAINT "PK_BuyOrder" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_BuyOrder_OrderBook_OrderBookId" FOREIGN KEY ("OrderBookId") REFERENCES "OrderBook"."OrderBook" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE TABLE "OrderBook"."SellOrder" (
        "Id" UUID NOT NULL,
        "OrderType" INTEGER NOT NULL,
        "OrderBookId" UUID NOT NULL,
        "StockSymbol" TEXT NOT NULL,
        "TraderId" TEXT NOT NULL,
        "Price" NUMERIC(18,6) NOT NULL,
        "Quantity" INTEGER NOT NULL,
        "Timestamp" TIMESTAMPTZ NOT NULL,
        "Status" INTEGER NOT NULL,
        CONSTRAINT "PK_SellOrder" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_SellOrder_OrderBook_OrderBookId" FOREIGN KEY ("OrderBookId") REFERENCES "OrderBook"."OrderBook" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE INDEX "IX_BuyOrder_OrderBookId" ON "OrderBook"."BuyOrder" ("OrderBookId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE INDEX "IX_BuyOrder_Timestamp" ON "OrderBook"."BuyOrder" ("Timestamp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE INDEX "IX_OrderBook_Ticker" ON "OrderBook"."OrderBook" ("Ticker");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE INDEX "IX_SellOrder_OrderBookId" ON "OrderBook"."SellOrder" ("OrderBookId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    CREATE INDEX "IX_SellOrder_Timestamp" ON "OrderBook"."SellOrder" ("Timestamp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250505162208_PopcornMakret.OrderBook.InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250505162208_PopcornMakret.OrderBook.InitialCreate', '9.0.3');
    END IF;
END $EF$;
COMMIT;

