To properly configure the initial migration script for support with TimeScale DB please add the following:

```
// Add Hypertable Support Only for PostgreSQL
if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
{
    migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb;");
    migrationBuilder.Sql("SELECT create_hypertable('OrderBook.Order', 'Timestamp', if_not_exists => TRUE);");
}
```

To run a migration and create an SQL script execute the following command:
```bash
dotnet ef migrations add PopcornMakret.OrderBook.MigrationName --project .\PopcornMarket.OrderBook.Persistence\PopcornMarket.OrderBook.Persistence.csproj
dotnet ef migrations script --project .\PopcornMarket.OrderBook.Persistence\PopcornMarket.OrderBook.Persistence.csproj --startup-project .\PopcornMarket.OrderBook.Api\PopcornMarket.OrderBook.Api.csproj --idempotent -o ./sql/scripts/PopcornMarket.OrderBook.Deploy.sql
```

The SQL scripts will be created in ./src/sql