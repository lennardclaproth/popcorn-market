CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'OrderBook') THEN
            CREATE SCHEMA "OrderBook";
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE TABLE "OrderBook"."Listing" (
        "Id" UUID NOT NULL,
        "Isin" TEXT NOT NULL,
        "Ticker" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Status" INTEGER NOT NULL,
        CONSTRAINT "PK_Listing" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE TABLE "OrderBook"."OrderBook" (
        "Id" UUID NOT NULL,
        "Ticker" TEXT NOT NULL,
        "ListingId" UUID NOT NULL,
        "CurrentPrice" NUMERIC(18,6),
        CONSTRAINT "PK_OrderBook" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_OrderBook_Listing_ListingId" FOREIGN KEY ("ListingId") REFERENCES "OrderBook"."Listing" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE TABLE "OrderBook"."Order" (
        "Id" UUID NOT NULL,
        "OrderBookId" UUID NOT NULL,
        "StockSymbol" TEXT NOT NULL,
        "TraderId" TEXT NOT NULL,
        "Price" NUMERIC(18,6) NOT NULL,
        "ExecutionPrice" NUMERIC(18,6) NOT NULL,
        "Quantity" INTEGER NOT NULL,
        "RemainingQuantity" INTEGER NOT NULL,
        "PlacedTimestamp" TIMESTAMPTZ NOT NULL,
        "ExecutedTimestamp" TIMESTAMPTZ,
        "Status" INTEGER NOT NULL,
        "OrderType" INTEGER NOT NULL,
        "OrderSide" INTEGER NOT NULL,
        CONSTRAINT "PK_Order" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Order_OrderBook_OrderBookId" FOREIGN KEY ("OrderBookId") REFERENCES "OrderBook"."OrderBook" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Order_ExecutedTimestamp" ON "OrderBook"."Order" ("ExecutedTimestamp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Order_OrderBookId" ON "OrderBook"."Order" ("OrderBookId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Order_PlacedTimestamp" ON "OrderBook"."Order" ("PlacedTimestamp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_OrderBook_ListingId" ON "OrderBook"."OrderBook" ("ListingId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_OrderBook_Ticker" ON "OrderBook"."OrderBook" ("Ticker");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250830152349_PopcornMarket.BabylonExchange.InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250830152349_PopcornMarket.BabylonExchange.InitialCreate', '9.0.3');
    END IF;
END $EF$;
COMMIT;

