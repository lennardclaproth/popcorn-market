package db

import (
	"fmt"

	"github.com/google/uuid"
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/internal/user"
)

type userStore struct {
	db *sqlx.DB
}

func NewUserStoreSqlx(db *sqlx.DB) *userStore {
	return &userStore{db: db}
}

func (us *userStore) Create(user *user.User) error {
	su := ToSchemaUser(user)

	query := fmt.Sprintf(`INSERT INTO %s (
		id, email, firstname, lastname, date_of_birth, password,
		created_at, street, city, state, zip_code, country
		) VALUES (
			:id, :email, :firstname, :lastname, :date_of_birth, :password,
			:created_at, :street, :city, :state, :zip_code, :country
		);`, TableUsers)

	_, err := us.db.NamedExec(query, su)

	return err
}

func (us *userStore) Delete(id uuid.UUID) error {
	panic("unimplemented")
}

func (us *userStore) GetByID(id uuid.UUID) (*user.User, error) {
	panic("unimplemented")
}

func (us *userStore) Update(user *user.User) error {
	panic("unimplemented")
}
