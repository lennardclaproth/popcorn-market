package application

import "time"

type CreateUserCommand struct {
	DateOfBirth time.Time
	Email,
	FirstName,
	LastName,
	Street,
	City,
	State,
	ZipCode,
	Country,
	Password string
}
