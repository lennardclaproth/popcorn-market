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
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
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
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ListingId = table.Column<Guid>(type: "UUID", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "NUMERIC(18,6)", precision: 18, scale: 6, nullable: true)
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
                name: "IX_OrderBook_ListingId",
                schema: "OrderBook",
                table: "OrderBook",
                column: "ListingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderBook_Ticker",
                schema: "OrderBook",
                table: "OrderBook",
                column: "Ticker");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
