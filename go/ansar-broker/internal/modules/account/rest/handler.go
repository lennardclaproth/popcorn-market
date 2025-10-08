// internal/account/adapter/http_handler.go
package rest

import (
	"encoding/json"
	"net/http"

	"github.com/go-chi/chi"
	"github.com/go-playground/validator/v10"
	"github.com/lennardclaproth/ansar-broker/internal/modules/account/usecase"
	"github.com/lennardclaproth/ansar-broker/internal/shared"
)

type Handler struct {
	// Define use cases
	logger   shared.Logger
	createUc *usecase.OpenAccount
	validate *validator.Validate
}

// Define use cases on new handler func
func NewHandler(logger shared.Logger, validate *validator.Validate, createUc *usecase.OpenAccount) *Handler {
	h := &Handler{
		logger:   logger,
		createUc: createUc,
	}

	return h
}

// RegisterRoutes wires routes into the default ServeMux
func (h *Handler) Routes() chi.Router {
	r := chi.NewRouter()

	r.Post("/", h.Create)

	return r
}

// Create godoc
// @Summary      Create account
// @Description  Creates a new account with initial balance
// @Tags         accounts
// @Accept       json
// @Produce      json
// @Param        input  body  OpenAccountRequest  true  "Account info"
// @Success      200  {object}  OpenAccountResponse
// @Router       /accounts [post]
func (h *Handler) Create(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "method not allowed", http.StatusMethodNotAllowed)
		return
	}

	var req OpenAccountRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "invalid request", http.StatusBadRequest)
		return
	}

	if err := h.validate.Struct(req); err != nil {
		http.Error(w, "validation failed: "+err.Error(), http.StatusBadRequest)
		return
	}

	c := usecase.OpenAccountCommand{
		UserId:  req.UserId,
		Balance: req.Balance,
	}

	if err := h.createUc.Execute(r.Context(), c); err != nil {
		h.logger.Error(r.Context(), "creation failed", err)
		http.Error(w, err.Error(), http.StatusInternalServerError)
		return
	}

	h.logger.Info(r.Context(), "creation successful")
	w.WriteHeader(http.StatusOK)
}
