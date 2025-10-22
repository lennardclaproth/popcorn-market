To properly configure the initial migration script for support with TimeScale DB please add the following:

```
// Add Hypertable Support Only for PostgreSQL
if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
{
    migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb;");
    migrationBuilder.Sql("SELECT create_hypertable('BabylonExchange.Order', 'Timestamp', if_not_exists => TRUE);");
}
```

To run a migration and create an SQL script execute the following command:
```bash
dotnet ef migrations add PopcornMarket.BabylonExchange.MigrationName --project .\PopcornMarket.BabylonExchange.Persistence\PopcornMarket.BabylonExchange.Persistence.csproj
dotnet ef migrations script --project .\PopcornMarket.BabylonExchange.Persistence\PopcornMarket.BabylonExchange.Persistence.csproj --startup-project .\PopcornMarket.BabylonExchange.Api\PopcornMarket.BabylonExchange.Api.csproj --idempotent -o ./sql/scripts/PopcornMarket.BabylonExchange.Migrations.sql
```

The SQL scripts will be created in ./src/sql