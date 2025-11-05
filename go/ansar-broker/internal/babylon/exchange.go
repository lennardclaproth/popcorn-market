package babylon

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"
	"time"

	"github.com/lennardclaproth/ansar-broker/config"
	httpx "github.com/lennardclaproth/ansar-broker/httpx/client"
	"github.com/lennardclaproth/ansar-broker/internal/order"
	"github.com/lennardclaproth/ansar-broker/internal/security"
)

type Service struct {
	client  *http.Client
	baseURL string
}

func NewService(cfg *config.Configuration) *Service {
	client := httpx.NewClient(
		httpx.WithCircuitBreaker(3, 3*time.Second),
	)

	return &Service{
		client:  client.Client,
		baseURL: cfg.Exchange.URI,
	}
}

// PlaceOrder sends an order to the exchange via a POST request
func (s *Service) PlaceOrder(ctx context.Context, o *order.Order) (string, error) {
	url := fmt.Sprintf("%s/api/v1/order", s.baseURL)
	// Build request payload based on the external API schema
	request := struct {
		TraderId string  `json:"trader_id"`
		Symbol   string  `json:"symbol"`
		Quantity int     `json:"quantity"`
		Price    float64 `json:"price"`
		Type     int     `json:"order_type"`
		Side     int     `json:"order_side"`
	}{
		TraderId: o.AccountID.String(), // assuming AccountID is the trader
		Symbol:   o.Ticker,
		Quantity: o.Quantity,
		Price:    o.Price,
		Type:     int(o.Type), // assuming enums are int-based
		Side:     int(o.Side),
	}

	// build request model
	body, err := json.Marshal(request)
	if err != nil {
		return "", err
	}

	// build new request
	req, err := http.NewRequestWithContext(ctx, http.MethodPost, url, bytes.NewBuffer(body))
	if err != nil {
		return "", err
	}

	req.Header.Set("Content-Type", "application/json")

	resp, err := s.client.Do(req)
	if err != nil {
		return "", err
	}

	// Make sure to close the reader of the response body.
	defer resp.Body.Close()

	// Check http statuscodes
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusCreated {
		return "", fmt.Errorf("unexpected response: %s", resp.Status)
	}

	var result struct {
		OrderID string `json:"order_id"`
	}

	err = json.NewDecoder(resp.Body).Decode(&result)

	if err != nil {
		return "", err
	}

	return result.OrderID, nil
}

// FindListings sends a GET request with a filter to the exchange.
func (s *Service) FindListings(ctx context.Context, f string, p, c int) ([]security.Listing, error) {
	url := fmt.Sprintf(`%s/api/v1/listing`, s.baseURL)

	// Build a request and set the query parameters of the request
	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, fmt.Errorf("new request: %w", err)
	}

	q := req.URL.Query()

	if f != "" {
		q.Add("filter", f)
	}
	q.Add("pageNumber", strconv.Itoa(p))
	q.Add("pageSize", strconv.Itoa(c))
	req.URL.RawQuery = q.Encode()
	req.Header.Add("Accept", "application/json")

	resp, err := s.client.Do(req)
	if err != nil {
		return nil, fmt.Errorf("send request: %w", err)
	}

	defer resp.Body.Close()

	// Execute request and map to result listing
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusCreated && resp.StatusCode != http.StatusNoContent {
		return nil, fmt.Errorf("unexpected response: %s", resp.Status)
	}

	var result struct {
		Listings []struct {
			Symbol          string    `json:"symbol"`
			Isin            string    `json:"isin"`
			CompanyName     string    `json:"company_name"`
			LastPrice       float64   `json:"last_price"`
			PriceOpen       float64   `json:"price_open"`
			PriceClose      float64   `json:"price_close"`
			PriceHigh       float64   `json:"price_high"`
			PriceLow        float64   `json:"price_low"`
			PriceChange     float64   `json:"price_change"`
			PriceChangePerc float64   `json:"price_change_perc"`
			Volume          int       `json:"volume"`
			LastUpdated     time.Time `json:"last_updated"`
		} `json:"listings"`
		PageCount  int `json:"pageCount"`
		PageNumber int `json:"pageNumber"`
		TotalCount int `json:"totalCount"`
	}

	err = json.NewDecoder(resp.Body).Decode(&result)
	if err != nil {
		return nil, err
	}

	listings := make([]security.Listing, 0, len(result.Listings))

	for _, listing := range result.Listings {
		listings = append(listings, security.Listing{
			Symbol:                listing.Symbol,
			Isin:                  listing.Isin,
			CompanyName:           listing.CompanyName,
			LastPrice:             listing.LastPrice,
			PriceOpen:             listing.PriceOpen,
			PriceClose:            listing.PriceClose,
			PriceHigh:             listing.PriceHigh,
			PriceLow:              listing.PriceLow,
			PriceChange:           listing.PriceChange,
			PriceChangePercentage: listing.PriceChangePerc,
			Volume:                int(listing.Volume),
			LastUpdated:           listing.LastUpdated,
		})
	}

	return listings, nil
}
