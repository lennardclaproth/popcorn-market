package http

import (
	"context"
	"fmt"
	"net/http"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/ansar-broker/errorx"
	"github.com/lennardclaproth/ansar-broker/httpx"
	"github.com/lennardclaproth/ansar-broker/internal/account"
	"github.com/lennardclaproth/ansar-broker/internal/order"
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
// @Success      201  {object}  openAccountResponse
// @Router       /api/v1/accounts/open [post]
func handleOpenAccount(accounts account.Service, log logging.Logger) http.HandlerFunc {
	return httpx.Handle(httpx.JSONDecoder[openAccountRequest],
		log,
		func(ctx context.Context, req openAccountRequest) (int, *openAccountResponse, error) {
			accountId, err := accounts.Open(ctx, account.OpenAccountCommand{UserID: req.UserId, InitialBalance: req.Balance})

			if err != nil {
				err = errorx.Trace(fmt.Errorf("failed to open account: %w", err))
				return 0, nil, err
			}

			return http.StatusCreated, &openAccountResponse{
				ID: accountId,
			}, nil
		})
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

	if !httpx.ValidateDateOnly(r.DateOfBirth) {
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
	return httpx.Handle(httpx.JSONDecoder[createUserRequest],
		log,
		func(ctx context.Context, req createUserRequest) (int, *createUserResponse, error) {
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

			userId, err := users.CreateUser(ctx, com)
			if err != nil {
				err = errorx.Trace(fmt.Errorf("failed to create user %w", err))
				return 0, nil, err
			}

			return http.StatusCreated, &createUserResponse{
				ID: userId.String(),
			}, nil
		})
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
func handleSearchSecurities(s security.Service, log logging.Logger) http.HandlerFunc {
	return httpx.Handle(httpx.QueryDecoder[SearchSecuritiesRequest],
		log,
		func(ctx context.Context, req SearchSecuritiesRequest) (int, []SearchSecuritiesResponse, error) {
			query := security.GetListingsQuery{
				Filter: req.Filter,
				Page:   req.Page,
				Count:  req.Count,
			}

			listings, err := s.SearchSecurities(ctx, query)
			if err != nil {
				return 0, nil, err
			}

			res := make([]SearchSecuritiesResponse, len(listings))
			for i, l := range listings {
				res[i] = SearchSecuritiesResponse{Symbol: l.Symbol, Name: l.CompanyName}
			}

			if len(res) == 0 {
				return http.StatusNoContent, nil, nil
			}

			return http.StatusOK, res, nil
		})
}

type OrderType int
type OrderSide int

const (
	OrderSideBuy OrderSide = iota
	OrderSideSell
)

const (
	OrderTypeMarket OrderType = iota
	OrderTypeLimit
)

// swagger:model
type placeOrderRequest struct {
	AccountID     uuid.UUID `json:"account_id"`
	AccountNumber string    `json:"account_number"`
	Ticker        string    `json:"ticker"`
	Quantity      int       `json:"quantity"`
	Price         float64   `json:"price"`
	Side          OrderSide `json:"order_side"` // Buy is 0 Sell is 1
	Type          OrderType `json:"order_type"` // Market is 0 Limit is 1
}

func (req *placeOrderRequest) Valid(ctx context.Context) map[string]string {
	problems := make(map[string]string)
	if req.Quantity <= 0 {
		problems["quantity"] = "quantity must be greater than zero"
	}

	// Check market order, market order cannot have a value.
	if req.Type == OrderTypeMarket && req.Price != 0 {
		problems["price"] = "price cannot have a value when placing a market order"
	}

	if req.Type == OrderTypeLimit && req.Price <= 0 {
		problems["price"] = "price cannot be smaller or equal to zero when placing a limit order"
	}

	return problems
}

type placeOrderResponse struct {
	OrderId string
}

// Create godoc
// @Summary      Place order
// @Description  Places a new order on an exchange
// @Tags         orders
// @Accept       json
// @Produce      json
// @Param        input  body  placeOrderRequest  true  "Place Order"
// @Success      200  {object}  placeOrderResponse
// @Failure      400  {object}  map[string]string
// @Failure      500  {object}  map[string]string
// @Router       /api/v1/orders/place [post]
func handlePlaceOrder(o order.Service, log logging.Logger) http.HandlerFunc {
	return httpx.Handle(httpx.JSONDecoder[placeOrderRequest],
		log,
		func(ctx context.Context, req placeOrderRequest) (int, *placeOrderResponse, error) {
			cmd := order.PlaceOrderCommand{
				AccountID:     req.AccountID,
				AccountNumber: req.AccountNumber,
				Quantity:      req.Quantity,
				Price:         req.Price,
				Ticker:        req.Ticker,
				Side:          order.OrderSide(req.Side),
				Type:          order.OrderType(req.Type),
			}

			res, err := o.PlaceOrder(ctx, cmd)
			if err != nil {
				err = errorx.Trace(err)
				return 0, nil, err
			}

			return http.StatusCreated, &placeOrderResponse{
				OrderId: res,
			}, nil
		})
}
