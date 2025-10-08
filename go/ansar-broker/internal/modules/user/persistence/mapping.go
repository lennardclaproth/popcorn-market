package persistence

import "github.com/lennardclaproth/ansar-broker/internal/modules/user/domain"

func SchemaUser(uu *domain.User) User {
	u := User{
		ID:          uu.ID,
		Email:       uu.Email,
		FirstName:   uu.FirstName,
		LastName:    uu.LastName,
		DateOfBirth: uu.DateOfBirth,
		Password:    uu.Password,
		CreatedAt:   uu.CreatedAt,
		Street:      uu.Address.Street,
		City:        uu.Address.City,
		State:       uu.Address.State,
		ZipCode:     uu.Address.ZipCode,
		Country:     uu.Address.Country,
	}

	return u
}

func DomainUser(u User) domain.User {
	a := domain.Address{
		Street:  u.Street,
		City:    u.City,
		State:   u.State,
		ZipCode: u.ZipCode,
		Country: u.Country,
	}

	uu := domain.User{
		ID:          u.ID,
		Email:       u.Email,
		FirstName:   u.FirstName,
		LastName:    u.LastName,
		DateOfBirth: u.DateOfBirth,
		Password:    u.Password,
		CreatedAt:   u.CreatedAt,
		Address:     a,
	}

	return uu
}
