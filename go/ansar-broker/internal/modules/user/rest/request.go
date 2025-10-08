package rest

// CreateUserRequest represents the request body for creating a user.
// swagger:model
type CreateUserRequest struct {
	DateOfBirth string `json:"date_of_birth" format:"date" example:"2000-01-31" validate:"required,dateonly"`
	Email       string `json:"email" example:"john.doe@example.com" validate:"required,email"`
	FirstName   string `json:"first_name" example:"John" validate:"min=3,max=255"`
	LastName    string `json:"last_name" example:"Doe" validate:"min=3,max=255"`
	Street      string `json:"street" example:"123 Main St" validate:"required"`
	City        string `json:"city" example:"Berlin" validate:"required"`
	State       string `json:"state" example:"Berlin" validate:"required"`
	ZipCode     string `json:"zip_code" example:"10115" validate:"required"`
	Country     string `json:"country" example:"Germany" validate:"required"`
	Password    string `json:"password" example:"SuperSecret123" validate:"required"`
}
