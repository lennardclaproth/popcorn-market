CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'OrderBook') THEN
            CREATE SCHEMA "OrderBook";
        END IF;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE TABLE "OrderBook"."Listing" (
        "Id" UUID NOT NULL,
        "Isin" TEXT NOT NULL,
        "StockSymbol" TEXT NOT NULL,
        "Name" TEXT NOT NULL,
        "Status" INTEGER NOT NULL,
        "PublicOfferingPrice" numeric(18,2) NOT NULL,
        "InitialPublicOfferingDate" timestamptz,
        "OpenPrice" numeric(18,2) NOT NULL,
        "HighPrice" numeric(18,2) NOT NULL,
        "LowPrice" numeric(18,2) NOT NULL,
        "ClosePrice" numeric(18,2) NOT NULL,
        "Volume" INTEGER NOT NULL,
        "LastUpdate" timestamptz NOT NULL,
        CONSTRAINT "PK_Listing" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE TABLE "OrderBook"."OrderBook" (
        "Id" UUID NOT NULL,
        "StockSymbol" TEXT NOT NULL,
        "ListingId" UUID NOT NULL,
        CONSTRAINT "PK_OrderBook" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_OrderBook_Listing_ListingId" FOREIGN KEY ("ListingId") REFERENCES "OrderBook"."Listing" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
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
        "StatusNote" TEXT,
        "OrderType" INTEGER NOT NULL,
        "OrderSide" INTEGER NOT NULL,
        CONSTRAINT "PK_Order" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Order_OrderBook_OrderBookId" FOREIGN KEY ("OrderBookId") REFERENCES "OrderBook"."OrderBook" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE TABLE "OrderBook"."Trade" (
        "Id" TEXT NOT NULL,
        "BuyOrderId" UUID NOT NULL,
        "SellOrderId" UUID NOT NULL,
        "StockSymbol" TEXT NOT NULL,
        "Price" numeric(18,2) NOT NULL,
        "Quantity" integer NOT NULL,
        "ExecutedAt" TEXT NOT NULL,
        CONSTRAINT "PK_Trade" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Trade_Order_BuyOrderId" FOREIGN KEY ("BuyOrderId") REFERENCES "OrderBook"."Order" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Trade_Order_SellOrderId" FOREIGN KEY ("SellOrderId") REFERENCES "OrderBook"."Order" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Listing_StockSymbol" ON "OrderBook"."Listing" ("StockSymbol");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Order_ExecutedTimestamp" ON "OrderBook"."Order" ("ExecutedTimestamp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Order_OrderBookId" ON "OrderBook"."Order" ("OrderBookId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Order_PlacedTimestamp" ON "OrderBook"."Order" ("PlacedTimestamp");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Order_StockSymbol" ON "OrderBook"."Order" ("StockSymbol");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_OrderBook_ListingId" ON "OrderBook"."OrderBook" ("ListingId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_OrderBook_StockSymbol" ON "OrderBook"."OrderBook" ("StockSymbol");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Trade_BuyOrderId" ON "OrderBook"."Trade" ("BuyOrderId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Trade_SellOrderId" ON "OrderBook"."Trade" ("SellOrderId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    CREATE INDEX "IX_Trade_StockSymbol" ON "OrderBook"."Trade" ("StockSymbol");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250910154102_PopcornMarket.BabylonExchange.InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250910154102_PopcornMarket.BabylonExchange.InitialCreate', '9.0.3');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022213803_PopcornMarket.BabylonExchange.OutboxPattern') THEN
    ALTER TABLE "OrderBook"."Order" ADD "OrderId" TEXT NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022213803_PopcornMarket.BabylonExchange.OutboxPattern') THEN
    CREATE TABLE "OutboxMessages" (
        "Id" UUID NOT NULL,
        "OccurredOnUtc" timestamptz NOT NULL,
        "Type" TEXT NOT NULL,
        "Payload" TEXT NOT NULL,
        "ProcessedOnUtc" timestamptz,
        CONSTRAINT "PK_OutboxMessages" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022213803_PopcornMarket.BabylonExchange.OutboxPattern') THEN
    CREATE UNIQUE INDEX "IX_Order_OrderId" ON "OrderBook"."Order" ("OrderId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251022213803_PopcornMarket.BabylonExchange.OutboxPattern') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20251022213803_PopcornMarket.BabylonExchange.OutboxPattern', '9.0.3');
    END IF;
END $EF$;
COMMIT;

