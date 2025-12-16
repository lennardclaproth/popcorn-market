package exchange

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"strconv"
	"time"

	"github.com/lennardclaproth/ansar-broker/internal/marketdata"
	"github.com/lennardclaproth/ansar-broker/internal/order"
	"github.com/lennardclaproth/ansar-broker/internal/security"
	orders "github.com/lennardclaproth/ansar-broker/protos"
	"go.elastic.co/apm/module/apmhttp/v2"
	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials"
)

type Client struct {
	http    *http.Client
	baseURL string
	grpcClient
}

type grpcClient struct {
	grpcConn   *grpc.ClientConn
	ordersGrpc orders.OrderServiceClient
}

type Option func(*Client)

type listingDTO struct {
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
}

type listingsResponse struct {
	Listings   []listingDTO `json:"listings"`
	PageCount  int          `json:"pageCount"`
	PageNumber int          `json:"pageNumber"`
	TotalCount int          `json:"totalCount"`
}

// withCircuitBreaker adds a circuit breaker to the http clien. This is an extension function
// that configures the client with extra options.
func WithCircuitBreaker(maxFailures int, resetTimeout time.Duration) Option {
	return func(c *Client) {
		c.http.Transport = NewCircuitBreaker(
			maxFailures,
			resetTimeout,
			c.http.Transport,
		)
	}
}

func WithGRPC(baseURL string) Option {
	return func(c *Client) {
		conn, err := grpc.NewClient(baseURL,
			grpc.WithDefaultCallOptions(grpc.WaitForReady(true)),
			grpc.WithTransportCredentials(credentials.NewClientTLSFromCert(nil, "")),
		)
		if err != nil {
			log.Fatalf("did not connect to: %v", err)
		}
		c.grpcConn = conn
		c.ordersGrpc = orders.NewOrderServiceClient(c.grpcConn)
	}
}

// NewClient creates a new exchange client.
func NewClient(baseURL string, opts ...Option) *Client {
	httpClient := &http.Client{
		Transport: apmhttp.WrapRoundTripper(http.DefaultTransport),
		Timeout:   10 * time.Second,
	}

	c := &Client{
		http:    httpClient,
		baseURL: baseURL,
	}

	for _, opt := range opts {
		opt(c)
	}

	return c
}

// PlaceOrder sends an order to the exchange via a POST request
func (s *Client) PlaceOrder(ctx context.Context, o *order.Order) (string, error) {
	req := &orders.PlaceOrderRequest{
		StockSymbol: o.Ticker,
		Side:        orders.OrderSide(o.Side),
		Type:        orders.OrderType(o.Type),
		Price:       o.Price,
		Quantity:    int32(o.Quantity),
		TraderId:    o.AccountNumber,
	}
	res, err := s.ordersGrpc.PlaceOrder(ctx, req)
	if err != nil {
		return "", fmt.Errorf("exchange: an error occurred while placing an order: %w", err)
	}

	return res.OrderId, nil
}

// FindListings sends a GET request with a filter to the exchange.
func (s *Client) FindListings(ctx context.Context, f string, p, c int) ([]security.Listing, error) {
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

	resp, err := s.http.Do(req)
	if err != nil {
		return nil, fmt.Errorf("send request: %w", err)
	}

	defer resp.Body.Close()

	// Execute request and map to result listing
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusCreated && resp.StatusCode != http.StatusNoContent {
		return nil, fmt.Errorf("unexpected response: %s", resp.Status)
	}

	var result listingsResponse

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

func (s *Client) Fetch(ctx context.Context, sym string) (*marketdata.Ticker, error) {
	url := fmt.Sprintf(`%s/api/v1/listing`, s.baseURL)

	// Build a request and set the query parameters of the request
	req, err := http.NewRequestWithContext(ctx, http.MethodGet, url, nil)
	if err != nil {
		return nil, fmt.Errorf("new request: %w", err)
	}

	q := req.URL.Query()

	if sym != "" {
		q.Add("filter", sym)
	}
	q.Add("pageNumber", strconv.Itoa(0))
	q.Add("pageSize", strconv.Itoa(1))
	req.URL.RawQuery = q.Encode()
	req.Header.Add("Accept", "application/json")

	resp, err := s.http.Do(req)
	if err != nil {
		return nil, fmt.Errorf("send request: %w", err)
	}

	defer resp.Body.Close()

	// Execute request and map to result listing
	if resp.StatusCode != http.StatusOK && resp.StatusCode != http.StatusCreated && resp.StatusCode != http.StatusNoContent {
		return nil, fmt.Errorf("unexpected response: %s", resp.Status)
	}

	var result listingsResponse

	err = json.NewDecoder(resp.Body).Decode(&result)
	if err != nil {
		return nil, err
	}

	l := result.Listings[0]
	return &marketdata.Ticker{
		Symbol:                l.Symbol,
		LastPrice:             l.LastPrice,
		PriceHigh:             l.PriceHigh,
		PriceLow:              l.PriceLow,
		PriceChange:           l.PriceChange,
		PriceChangePercentage: l.PriceChangePerc,
		Volume:                l.Volume,
		LastUpdated:           l.LastUpdated,
	}, nil
}
