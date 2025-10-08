package rest

import (
	"encoding/json"
	"net/http"
	"time"

	"github.com/go-chi/chi"
	"github.com/go-playground/validator/v10"
	"github.com/lennardclaproth/ansar-broker/internal/modules/user/application"
	"github.com/lennardclaproth/ansar-broker/internal/shared"
)

type Handler struct {
	logger       shared.Logger
	userUseCases application.Services
	validate     *validator.Validate
}

func NewHandler(logger shared.Logger, userUseCases application.Services, validate *validator.Validate) *Handler {
	h := &Handler{
		logger:       logger,
		userUseCases: userUseCases,
		validate:     validate,
	}

	return h
}

func (h *Handler) Routes() chi.Router {
	r := chi.NewRouter()

	r.Post("/", h.Create)

	return r
}

// Create godoc
// @Summary      Create user
// @Description  Creates a new user
// @Tags         users
// @Accept       json
// @Produce      json
// @Param        input  body  CreateUserRequest  true  "Create User"
// @Success      200  {object}  CreateUserResponse
// @Router       /users [post]
func (h *Handler) Create(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		return
	}

	var req CreateUserRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "invalid request", http.StatusBadRequest)
		return
	}

	if err := h.validate.Struct(req); err != nil {
		http.Error(w, "validation failed: "+err.Error(), http.StatusBadRequest)
		return
	}

	dob, _ := time.Parse(time.DateOnly, req.DateOfBirth)

	c := application.CreateUserCommand{
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

	if _, err := h.userUseCases.UserService.CreateUser(r.Context(), c); err != nil {
		h.logger.Error(r.Context(), "creation failed", err)
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	h.logger.Info(r.Context(), "creation successful")
	w.WriteHeader(http.StatusOK)
}
