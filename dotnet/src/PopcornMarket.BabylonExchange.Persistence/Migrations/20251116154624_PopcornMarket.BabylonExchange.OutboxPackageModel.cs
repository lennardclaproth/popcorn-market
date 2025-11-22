using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PopcornMarket.BabylonExchange.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopcornMarketBabylonExchangeOutboxPackageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "OutboxMessages",
                newName: "Topic");

            migrationBuilder.RenameColumn(
                name: "ProcessedOnUtc",
                table: "OutboxMessages",
                newName: "ProcessedAtUtc");

            migrationBuilder.RenameColumn(
                name: "OccurredOnUtc",
                table: "OutboxMessages",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<int>(
                name: "Attempts",
                table: "OutboxMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "OutboxMessages",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attempts",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "OutboxMessages",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ProcessedAtUtc",
                table: "OutboxMessages",
                newName: "ProcessedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "OutboxMessages",
                newName: "OccurredOnUtc");
        }
    }
}
