To properly configure the initial migration script for support with TimeScale DB please add the following:

```
// Add Hypertable Support Only for PostgreSQL
if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
{
    migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb;");
    migrationBuilder.Sql("SELECT create_hypertable('OrderBook.Order', 'Timestamp', if_not_exists => TRUE);");
}
```