package persistence

import (
	"fmt"

	"github.com/google/uuid"
	"github.com/jmoiron/sqlx"
	"github.com/lennardclaproth/ansar-broker/internal/modules/user/domain"
)

type userRepository struct {
	db *sqlx.DB
}

func NewUserRepositorySqlx(db *sqlx.DB) *userRepository {
	return &userRepository{db: db}
}

func (ur *userRepository) Create(user *domain.User) error {
	su := SchemaUser(user)

	query := fmt.Sprintf(`INSERT INTO %s (
		id, email, firstname, lastname, date_of_birth, password,
		created_at, street, city, state, zip_code, country
		) VALUES (
			:id, :email, :firstname, :lastname, :date_of_birth, :password,
			:created_at, :street, :city, :state, :zip_code, :country
		);`, TableUsers)

	_, err := ur.db.NamedExec(query, su)

	return err
}

func (ur *userRepository) Delete(id uuid.UUID) error {
	panic("unimplemented")
}

func (ur *userRepository) GetByID(id uuid.UUID) (*domain.User, error) {
	panic("unimplemented")
}

func (ur *userRepository) Update(user *domain.User) error {
	panic("unimplemented")
}
