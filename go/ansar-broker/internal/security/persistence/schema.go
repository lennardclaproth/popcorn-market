package persistence

type Security struct {
	Symbol string `db:"symbol"`
	Name   string `db:"name"`
}
