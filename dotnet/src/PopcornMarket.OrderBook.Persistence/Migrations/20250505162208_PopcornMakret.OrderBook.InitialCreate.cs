using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PopcornMarket.OrderBook.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopcornMakretOrderBookInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "OrderBook");

            migrationBuilder.CreateTable(
                name: "Company",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    StockPriceUSD = table.Column<decimal>(type: "NUMERIC(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderBook",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    Ticker = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BuyOrder",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    OrderType = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderBookId = table.Column<Guid>(type: "UUID", nullable: false),
                    StockSymbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    TraderId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "NUMERIC(18,6)", precision: 18, scale: 6, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyOrder_OrderBook_OrderBookId",
                        column: x => x.OrderBookId,
                        principalSchema: "OrderBook",
                        principalTable: "OrderBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellOrder",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    OrderType = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderBookId = table.Column<Guid>(type: "UUID", nullable: false),
                    StockSymbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    TraderId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "NUMERIC(18,6)", precision: 18, scale: 6, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellOrder_OrderBook_OrderBookId",
                        column: x => x.OrderBookId,
                        principalSchema: "OrderBook",
                        principalTable: "OrderBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyOrder_OrderBookId",
                schema: "OrderBook",
                table: "BuyOrder",
                column: "OrderBookId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyOrder_Timestamp",
                schema: "OrderBook",
                table: "BuyOrder",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBook_Ticker",
                schema: "OrderBook",
                table: "OrderBook",
                column: "Ticker");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrder_OrderBookId",
                schema: "OrderBook",
                table: "SellOrder",
                column: "OrderBookId");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrder_Timestamp",
                schema: "OrderBook",
                table: "SellOrder",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyOrder",
                schema: "OrderBook");

            migrationBuilder.DropTable(
                name: "Company",
                schema: "OrderBook");

            migrationBuilder.DropTable(
                name: "SellOrder",
                schema: "OrderBook");

            migrationBuilder.DropTable(
                name: "OrderBook",
                schema: "OrderBook");
        }
    }
}
