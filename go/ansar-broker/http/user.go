package http

import (
	"context"
	"net/http"
	"time"

	"github.com/lennardclaproth/ansar-broker/internal/user"
	"github.com/lennardclaproth/ansar-broker/logging"
)

// createUserRequest represents the request body for creating a user.
// swagger:model
type createUserRequest struct {
	DateOfBirth string `json:"date_of_birth" format:"date" example:"2000-01-31"`
	Email       string `json:"email" example:"john.doe@example.com"`
	FirstName   string `json:"first_name" example:"John"`
	LastName    string `json:"last_name" example:"Doe"`
	Street      string `json:"street" example:"123 Main St"`
	City        string `json:"city" example:"Berlin"`
	State       string `json:"state" example:"Berlin"`
	ZipCode     string `json:"zip_code" example:"10115"`
	Country     string `json:"country" example:"Germany"`
	Password    string `json:"password" example:"SuperSecret123"`
}

func (r createUserRequest) Valid(ctx context.Context) map[string]string {
	problems := make(map[string]string)

	if !ValidateDateOnly(r.DateOfBirth) {
		problems["date_of_birth"] = "date of birth is not in a valid date only format."
	}

	if len(r.FirstName) < 3 || len(r.FirstName) > 255 {
		problems["first_name"] = "first name must be between 3 and 255 characters"
	}

	return problems
}

type createUserResponse struct {
	ID string `json:"id" example:"123e4567-e89b-12d3-a456-426614174000"`
}

// Create godoc
// @Summary      Create user
// @Description  Creates a new user
// @Tags         users
// @Accept       json
// @Produce      json
// @Param        input  body  createUserRequest  true  "Create User"
// @Success      200  {object}  createUserResponse
// @Router       /api/v1/users/create [post]
func handleCreateUser(users user.Facade, log logging.Logger) http.HandlerFunc {

	return func(w http.ResponseWriter, r *http.Request) {
		req, problems, err := decodeValid[createUserRequest](r)
		if err != nil && problems != nil {
			encode(w, http.StatusBadRequest, problems)
		}

		if err != nil {
			encode(w, http.StatusBadRequest, map[string]string{"error": err.Error()})
		}

		dob, _ := time.Parse(time.DateOnly, req.DateOfBirth)
		com := user.CreateUserCommand{
			DateOfBirth: dob,
			Email:       req.Email,
			FirstName:   req.FirstName,
			LastName:    req.LastName,
			Street:      req.Street,
			City:        req.City,
			State:       req.State,
			ZipCode:     req.ZipCode,
			Country:     req.Country,
			Password:    req.Password,
		}

		userId, err := users.UserService.CreateUser(r.Context(), com)
		if err != nil {
			log.Error(r.Context(), "creation failed", err)
			_ = encode(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
			return
		}

		_ = encode(w, http.StatusCreated, createUserResponse{
			ID: userId.String(),
		})
	}
}
