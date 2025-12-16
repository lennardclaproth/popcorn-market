using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PopcornMarket.BabylonExchange.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopcornMarketBabylonExchangeTradeCorrectColumnValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "OrderBook",
                table: "Trade",
                type: "numeric(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecutedAt",
                schema: "OrderBook",
                table: "Trade",
                type: "TIMESTAMPTZ",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "OrderBook",
                table: "Trade",
                type: "UUID",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                schema: "OrderBook",
                table: "Trade",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecutedAt",
                schema: "OrderBook",
                table: "Trade",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMPTZ");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                schema: "OrderBook",
                table: "Trade",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "UUID");
        }
    }
}
