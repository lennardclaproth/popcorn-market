package user

import (
	"time"

	"github.com/google/uuid"
)

type User struct {
	ID          uuid.UUID
	Email       string
	FirstName   string
	LastName    string
	DateOfBirth time.Time
	Address     Address
	Password    string
	CreatedAt   time.Time
	Accounts    []uuid.UUID
}

type Address struct {
	Street  string
	City    string
	State   string
	ZipCode string
	Country string
}

func NewUser(dateOfBirth time.Time, email, firstName, lastName, street, city, state, zipCode, country, password string) *User {
	return &User{
		ID:          uuid.New(),
		Email:       email,
		FirstName:   firstName,
		LastName:    lastName,
		DateOfBirth: dateOfBirth,
		Address: Address{
			Street:  street,
			City:    city,
			State:   state,
			ZipCode: zipCode,
			Country: country,
		},
		Password:  password,
		CreatedAt: time.Now(),
		Accounts:  []uuid.UUID{},
	}
}
