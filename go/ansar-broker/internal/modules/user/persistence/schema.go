package persistence

import (
	"time"

	"github.com/google/uuid"
)

const (
	SchemaMain = "ansar"
	TableUsers = "users"
)

type User struct {
	ID          uuid.UUID `db:"id"`
	Email       string    `db:"email"`
	FirstName   string    `db:"firstname"`
	LastName    string    `db:"lastname"`
	DateOfBirth time.Time `db:"date_of_birth"`
	Password    string    `db:"password"`
	CreatedAt   time.Time `db:"created_at"`
	Street      string    `db:"street"`
	City        string    `db:"city"`
	State       string    `db:"state"`
	ZipCode     string    `db:"zip_code"`
	Country     string    `db:"country"`
}
