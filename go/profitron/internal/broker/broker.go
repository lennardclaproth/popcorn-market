package broker

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/profitron/errorx"
)

// Broker is an HTTP client that communicates with the Ansar-Broker service.
type Broker struct {
	BaseURL    string
	HTTPClient *http.Client
}

// NewBroker returns a new Broker instance.
func NewBroker(baseURL string) *Broker {
	return &Broker{
		BaseURL: baseURL,
		HTTPClient: &http.Client{
			Timeout: 10 * time.Second,
		},
	}
}

// --------------------------------------------------------
// USERS
// --------------------------------------------------------

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
	url := fmt.Sprintf("%s/api/v1/users/create", b.BaseURL)
	body, _ := json.Marshal(req)

	request, err := http.NewRequestWithContext(ctx, http.MethodPost, url, bytes.NewReader(body))
	if err != nil {
		return uuid.Nil, errorx.Trace(fmt.Errorf("failed to create request: %w", err))
	}
	request.Header.Set("Content-Type", "application/json")

	resp, err := b.HTTPClient.Do(request)
	if err != nil {
		return uuid.Nil, errorx.Trace(fmt.Errorf("failed to send request: %w", err))
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusCreated && resp.
		StatusCode != http.StatusOK {
		defer resp.Body.Close()

		body, _ := io.ReadAll(resp.Body)

		// Try to decode a structured validation error
		var validation map[string]string
		if err := json.Unmarshal(body, &validation); err == nil && len(validation) > 0 {
			// Found a validation error map -> return a detailed error message
			return uuid.Nil, errorx.Trace(fmt.Errorf("validation failed (status %d): %v", resp.StatusCode, validation))
		}

		// Otherwise just include raw body text for debugging
		return uuid.Nil, errorx.Trace(fmt.Errorf("unexpected status %d, body: %s", resp.StatusCode, bytes.TrimSpace(body)))
	}

	var res CreateUserResponse
	if err := json.NewDecoder(resp.Body).Decode(&res); err != nil {
		return uuid.Nil, fmt.Errorf("failed to decode response: %w", err)
	}

	return uuid.Parse(res.ID)
}

// --------------------------------------------------------
// ACCOUNTS
// --------------------------------------------------------

type OpenAccountRequest struct {
	UserID  uuid.UUID `json:"uid"`
	Balance float64   `json:"balance"`
}

type OpenAccountResponse struct {
	ID string `json:"id"`
}

// OpenAccount calls POST /api/v1/accounts/open
func (b *Broker) OpenAccount(ctx context.Context, userID uuid.UUID, balance float64) (string, error) {
	url := fmt.Sprintf("%s/api/v1/accounts/open", b.BaseURL)
	body, _ := json.Marshal(OpenAccountRequest{UserID: userID, Balance: balance})

	req, err := http.NewRequestWithContext(ctx, http.MethodPost, url, bytes.NewReader(body))
	if err != nil {
		return "", fmt.Errorf("failed to create request: %w", err)
	}
	req.Header.Set("Content-Type", "application/json")

	resp, err := b.HTTPClient.Do(req)
	if err != nil {
		return "", fmt.Errorf("failed to send request: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusCreated && resp.StatusCode != http.StatusOK {
		return "", fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	var res OpenAccountResponse
	if err := json.NewDecoder(resp.Body).Decode(&res); err != nil {
		return "", fmt.Errorf("failed to decode response: %w", err)
	}

	return res.ID, nil
}

// --------------------------------------------------------
// SECURITIES
// --------------------------------------------------------

type SearchSecuritiesResponse struct {
	Symbol string `json:"symbol"`
	Name   string `json:"name"`
}

// FetchSecurities calls GET /api/v1/securities/search
func (b *Broker) FetchSecurities(ctx context.Context, filter string, page, count int) ([]SearchSecuritiesResponse, error) {
	url := fmt.Sprintf("%s/api/v1/securities/search?filter=%s&page=%d&count=%d", b.BaseURL, filter, page, count)

	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, fmt.Errorf("failed to create request: %w", err)
	}

	resp, err := b.HTTPClient.Do(req)
	if err != nil {
		return nil, fmt.Errorf("failed to send request: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode == http.StatusNoContent {
		return []SearchSecuritiesResponse{}, nil
	}
	if resp.StatusCode != http.StatusOK {
		return nil, fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	var res []SearchSecuritiesResponse
	if err := json.NewDecoder(resp.Body).Decode(&res); err != nil {
		return nil, fmt.Errorf("failed to decode response: %w", err)
	}

	return res, nil
}

// --------------------------------------------------------
// ORDERS
// --------------------------------------------------------

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
	url := fmt.Sprintf("%s/api/v1/orders/place", b.BaseURL)
	body, _ := json.Marshal(req)

	httpReq, err := http.NewRequestWithContext(ctx, http.MethodPost, url, bytes.NewReader(body))
	if err != nil {
		return "", fmt.Errorf("failed to create request: %w", err)
	}
	httpReq.Header.Set("Content-Type", "application/json")

	resp, err := b.HTTPClient.Do(httpReq)
	if err != nil {
		return "", fmt.Errorf("failed to send request: %w", err)
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusCreated && resp.StatusCode != http.StatusOK {
		return "", fmt.Errorf("unexpected status code: %d", resp.StatusCode)
	}

	var res PlaceOrderResponse
	if err := json.NewDecoder(resp.Body).Decode(&res); err != nil {
		return "", fmt.Errorf("failed to decode response: %w", err)
	}

	return res.OrderID, nil
}
