using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PopcornMarket.OrderBook.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopcornOrderBookInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "OrderBook");

            migrationBuilder.CreateTable(
                name: "OrderBook",
                schema: "OrderBook",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "UUID", nullable: false),
                    StockSymbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderBook", x => x.Id);
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
                    Price = table.Column<decimal>(type: "NUMERIC(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderType = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false)
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

            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb;");
                migrationBuilder.Sql("SELECT create_hypertable('OrderBook.Order', 'Timestamp', if_not_exists => TRUE);");
            }
            
            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderBookId",
                schema: "OrderBook",
                table: "Order",
                column: "OrderBookId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Timestamp",
                schema: "OrderBook",
                table: "Order",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_OrderBook_StockSymbol",
                schema: "OrderBook",
                table: "OrderBook",
                column: "StockSymbol");
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
        }
    }
}
