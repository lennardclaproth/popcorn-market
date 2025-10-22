using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PopcornMarket.BabylonExchange.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopcornMarketBabylonExchangeOutboxUUID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "OutboxMessages",
                type: "UUID",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "OutboxMessages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "UUID");
        }
    }
}
