package http

import (
	"context"
	"net/http"
	"strconv"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/internal/security"
	"github.com/lennardclaproth/ansar-broker/internal/user"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type openAccountRequest struct {
	UserId  uuid.UUID `json:"uid"`
	Balance float64   `json:"balance"`
}

type openAccountResponse struct {
	ID string `json:"id"`
}

// Implement the Validator interface for this specific request type
func (r openAccountRequest) Valid(ctx context.Context) map[string]string {
	problems := make(map[string]string)

	if r.UserId == uuid.Nil {
		problems["uid"] = "user ID is required"
	}
	if r.Balance < 0 {
		problems["balance"] = "balance cannot be negative"
	}

	return problems
}

// Create godoc
// @Summary      Create account
// @Description  Creates a new account with initial balance
// @Tags         accounts
// @Accept       json
// @Produce      json
// @Param        input  body  openAccountRequest  true  "Account info"
// @Success      200  {object}  openAccountResponse
// @Router       /api/v1/accounts/open [post]
func handleOpenAccount(accounts account.Service, log logging.Logger) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		req, problems, err := decodeValid[openAccountRequest](r)
		if err != nil && problems != nil {
			encode(w, http.StatusBadRequest, problems)
		}

		if err != nil {
			encode(w, http.StatusBadRequest, map[string]string{"error": err.Error()})
		}
		accountId, err := accounts.Open(r.Context(), account.OpenAccountCommand{UserID: req.UserId, InitialBalance: req.Balance})

		if err != nil {
			log.Error(r.Context(), "creation failed", err)
			_ = encode(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
			return
		}

		_ = encode(w, http.StatusCreated, openAccountResponse{
			ID: accountId.String(),
		})
	}
}

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
func handleCreateUser(users user.Service, log logging.Logger) http.HandlerFunc {
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

		userId, err := users.CreateUser(r.Context(), com)
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

type SearchSecuritiesRequest struct {
	Filter string `json:"filter" form:"filter" query:"filter"`
	Page   int    `json:"page" form:"page" query:"page"`
	Count  int    `json:"count" form:"count" query:"count"`
}

type SearchSecuritiesResponse struct {
	Symbol string `json:"symbol"`
	Name   string `json:"name"`
}

// handleSearchSecurities godoc
// @Summary      Search securities
// @Description  Retrieves a paginated list of securities matching the filter
// @Tags         securities
// @Accept       json
// @Produce      json
// @Param        filter  query  string  false  "Filter by company or symbol"
// @Param        page    query  int     true   "Page number"   default(1)
// @Param        count   query  int     true   "Items per page" default(10)
// @Success      200  {array}  SearchSecuritiesResponse
// @Failure      400  {object}  map[string]string
// @Failure      500  {object}  map[string]string
// @Router       /api/v1/securities/search [get]
func (r *SearchSecuritiesRequest) Valid(ctx context.Context) map[string]string {
	problems := make(map[string]string)

	if r.Page <= 0 {
		problems["page"] = "page must be greater than 0"
	}
	if r.Count <= 0 || r.Count > 100 {
		problems["count"] = "count must be between 1 and 100"
	}

	if len(problems) == 0 {
		return nil
	}
	return problems
}

func handleSearchSecurities(s security.Service, log logging.Logger) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		// Parse query parameters with sensible defaults
		page, _ := strconv.Atoi(r.URL.Query().Get("page"))
		count, _ := strconv.Atoi(r.URL.Query().Get("count"))

		req := SearchSecuritiesRequest{
			Filter: r.URL.Query().Get("filter"),
			Page:   page,
			Count:  count,
		}

		if problems := req.Valid(r.Context()); problems != nil {
			encode(w, http.StatusBadRequest, problems)
			return
		}

		query := security.GetListingsQuery{
			Filter: req.Filter,
			Page:   req.Page,
			Count:  req.Count,
		}

		listings, err := s.SearchSecurities(r.Context(), query)
		if err != nil {
			log.Error(r.Context(), "search securities", err)
			encode(w, http.StatusInternalServerError, map[string]string{"error": "internal server error"})
			return
		}

		res := make([]SearchSecuritiesResponse, 0, len(listings))
		for _, l := range listings {
			res = append(res, SearchSecuritiesResponse{
				Symbol: l.Symbol,
				Name:   l.CompanyName,
			})
		}

		encode(w, http.StatusOK, res)
	}
}
