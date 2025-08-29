# popcorn-market

A fun custom market simulation project

---

## Naming conventions

### Databases

1. MongoDB
   1. Databases, database names start with the popcorn prefix after which the application name follows to finish with the postfix DB, i.e. popcornFinancialAtlasDB where the application name is CamelCase
   2. Collections represent the name of the domain models i.e. the entities or value objects in snakecase
2. SQL (Postgres)
   1. Databases are represented by popcorn_ApplicationNameDB.
   2. The table names differ between .NET EFCore and Spring Hibernate because of how hibernate translates the names of tables. Hibernate changes all table names to lower case. therefore we use snakecase
