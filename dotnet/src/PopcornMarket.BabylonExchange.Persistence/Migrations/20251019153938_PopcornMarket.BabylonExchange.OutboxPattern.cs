using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PopcornMarket.BabylonExchange.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopcornMarketBabylonExchangeOutboxPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                schema: "OrderBook",
                table: "Order",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Payload = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderId",
                schema: "OrderBook",
                table: "Order",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderId",
                schema: "OrderBook",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "OrderBook",
                table: "Order");
        }
    }
}
