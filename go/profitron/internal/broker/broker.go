package broker

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/profitron/errorx"
	"github.com/lennardclaproth/profitron/httpx"
)

// Broker is an HTTP client that communicates with the Ansar-Broker service.
type Broker struct {
	baseURI string
	client  *http.Client
}

// NewBroker returns a new Broker instance.
func NewBroker(baseURI string) *Broker {
	return &Broker{
		baseURI: baseURI,
		client: &http.Client{
			Timeout: 10 * time.Second,
		},
	}
}

type CreateUserRequest struct {
	DateOfBirth string `json:"date_of_birth"`
	Email       string `json:"email"`
	FirstName   string `json:"first_name"`
	LastName    string `json:"last_name"`
	Street      string `json:"street"`
	City        string `json:"city"`
	State       string `json:"state"`
	ZipCode     string `json:"zip_code"`
	Country     string `json:"country"`
	Password    string `json:"password"`
}

type CreateUserResponse struct {
	ID string `json:"id"`
}

// CreateUser calls POST /api/v1/users/create
func (b *Broker) CreateUser(ctx context.Context, req CreateUserRequest) (uuid.UUID, error) {
	url := fmt.Sprintf("%s/api/v1/users/create", b.baseURI)
	body, _ := json.Marshal(req)

	request, err := http.NewRequestWithContext(ctx, http.MethodPost, url, bytes.NewReader(body))
	if err != nil {
		return uuid.Nil, errorx.Trace(fmt.Errorf("failed to create request: %w", err))
	}
	request.Header.Set("Content-Type", "application/json")

	resp, err := b.client.Do(request)
	if err != nil {
		return uuid.Nil, errorx.Trace(fmt.Errorf("failed to send request: %w", err))
	}

	res, err := httpx.DecodeJSONResponse[CreateUserResponse](resp)
	if err != nil {
		return uuid.Nil, errorx.Trace(fmt.Errorf("broker: failed to create a user: %w", err))
	}
	return uuid.Parse(res.ID)
}

type OpenAccountRequest struct {
	UserID  uuid.UUID `json:"uid"`
	Balance float64   `json:"balance"`
}

type OpenAccountResponse struct {
	ID string `json:"id"`
}

// OpenAccount calls POST /api/v1/accounts/open
func (b *Broker) OpenAccount(ctx context.Context, userID uuid.UUID, balance float64) (string, error) {
	url := fmt.Sprintf("%s/api/v1/accounts/open", b.baseURI)
	body, _ := json.Marshal(OpenAccountRequest{UserID: userID, Balance: balance})

	req, err := http.NewRequestWithContext(ctx, http.MethodPost, url, bytes.NewReader(body))
	if err != nil {
		return "", errorx.Trace(fmt.Errorf("broker: failed to create request to open an account: %w", err))
	}
	req.Header.Set("Content-Type", "application/json")

	resp, err := b.client.Do(req)
	if err != nil {
		return "", errorx.Trace(fmt.Errorf("broker: failed to send request to open an account: %w", err))
	}

	res, err := httpx.DecodeJSONResponse[OpenAccountResponse](resp)
	if err != nil {
		return "", errorx.Trace(fmt.Errorf("broker: failed to open an account: %w", err))
	}

	return res.ID, nil
}

type SearchSecuritiesResponse struct {
	Symbol string `json:"symbol"`
	Name   string `json:"name"`
}

// FetchSecurities calls GET /api/v1/securities/search
func (b *Broker) FetchSecurities(ctx context.Context, filter string, page, count int) ([]SearchSecuritiesResponse, error) {
	url := fmt.Sprintf("%s/api/v1/securities/search?filter=%s&page=%d&count=%d", b.baseURI, filter, page, count)

	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to create request: %w", err)
	}

	resp, err := b.client.Do(req)
	if err != nil {
		return nil, errorx.Trace(fmt.Errorf("broker: failed to perform request: %w", err))
	}

	res, err := httpx.DecodeJSONResponse[[]SearchSecuritiesResponse](resp)
	if err != nil {
		return nil, errorx.Trace(fmt.Errorf("broker: failed to fetch securities: %w", err))
	}
	return res, nil
}

type PlaceOrderRequest struct {
	AccountID     uuid.UUID `json:"account_id"`
	AccountNumber string    `json:"account_number"`
	Ticker        string    `json:"ticker"`
	Quantity      int       `json:"quantity"`
	Price         float64   `json:"price"`
	OrderSide     int       `json:"order_side"`
	OrderType     int       `json:"order_type"`
}

type PlaceOrderResponse struct {
	OrderID string `json:"order_id"`
}

// PlaceOrder calls POST /api/v1/orders/place
func (b *Broker) PlaceOrder(ctx context.Context, req PlaceOrderRequest) (string, error) {
	url := fmt.Sprintf("%s/api/v1/orders/place", b.baseURI)
	body, _ := json.Marshal(req)

	httpReq, err := http.NewRequestWithContext(ctx, http.MethodPost, url, bytes.NewReader(body))
	if err != nil {
		return "", fmt.Errorf("failed to create request: %w", err)
	}
	httpReq.Header.Set("Content-Type", "application/json")

	resp, err := b.client.Do(httpReq)
	if err != nil {
		return "", errorx.Trace(fmt.Errorf("broker: failed to send request: %w", err))
	}

	res, err := httpx.DecodeJSONResponse[PlaceOrderResponse](resp)
	if err != nil {
		return "", errorx.Trace(fmt.Errorf("broker: place order failed: %w", err))
	}

	return res.OrderID, nil
}
