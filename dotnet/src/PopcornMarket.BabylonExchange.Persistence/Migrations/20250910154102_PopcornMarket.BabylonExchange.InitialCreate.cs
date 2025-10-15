using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PopcornMarket.BabylonExchange.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopcornMarketBabylonExchangeInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "OrderBook");

            migrationBuilder.CreateTable(
                name: "Listing",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    Isin = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    StockSymbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicOfferingPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    InitialPublicOfferingDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    OpenPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    HighPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LowPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Volume = table.Column<long>(type: "INTEGER", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderBook",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    StockSymbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ListingId = table.Column<Guid>(type: "UUID", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderBook_Listing_ListingId",
                        column: x => x.ListingId,
                        principalSchema: "OrderBook",
                        principalTable: "Listing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    OrderBookId = table.Column<Guid>(type: "UUID", nullable: false),
                    StockSymbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    TraderId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "NUMERIC(18,6)", precision: 18, scale: 6, nullable: false),
                    ExecutionPrice = table.Column<decimal>(type: "NUMERIC(18,6)", precision: 18, scale: 6, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    RemainingQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    PlacedTimestamp = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    ExecutedTimestamp = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusNote = table.Column<string>(type: "TEXT", nullable: true),
                    OrderType = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderSide = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_OrderBook_OrderBookId",
                        column: x => x.OrderBookId,
                        principalSchema: "OrderBook",
                        principalTable: "OrderBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trade",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BuyOrderId = table.Column<Guid>(type: "UUID", nullable: false),
                    SellOrderId = table.Column<Guid>(type: "UUID", nullable: false),
                    StockSymbol = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trade_Order_BuyOrderId",
                        column: x => x.BuyOrderId,
                        principalSchema: "OrderBook",
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trade_Order_SellOrderId",
                        column: x => x.SellOrderId,
                        principalSchema: "OrderBook",
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listing_StockSymbol",
                schema: "OrderBook",
                table: "Listing",
                column: "StockSymbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_ExecutedTimestamp",
                schema: "OrderBook",
                table: "Order",
                column: "ExecutedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderBookId",
                schema: "OrderBook",
                table: "Order",
                column: "OrderBookId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PlacedTimestamp",
                schema: "OrderBook",
                table: "Order",
                column: "PlacedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Order_StockSymbol",
                schema: "OrderBook",
                table: "Order",
                column: "StockSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBook_ListingId",
                schema: "OrderBook",
                table: "OrderBook",
                column: "ListingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderBook_StockSymbol",
                schema: "OrderBook",
                table: "OrderBook",
                column: "StockSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_BuyOrderId",
                schema: "OrderBook",
                table: "Trade",
                column: "BuyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_SellOrderId",
                schema: "OrderBook",
                table: "Trade",
                column: "SellOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_StockSymbol",
                schema: "OrderBook",
                table: "Trade",
                column: "StockSymbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trade",
                schema: "OrderBook");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "OrderBook");

            migrationBuilder.DropTable(
                name: "OrderBook",
                schema: "OrderBook");

            migrationBuilder.DropTable(
                name: "Listing",
                schema: "OrderBook");
        }
    }
}
