package migrations

import "embed"

//go:embed sqlite/*.sql
var SQLite embed.FS
