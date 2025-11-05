package httpx

import (
	"net/http"
	"time"

	"go.elastic.co/apm/module/apmhttp/v2"
)

type Client struct {
	*http.Client
}

type ClientOption func(*Client)

// withCircuitBreaker adds a circuit breaker to the http clien. This is an extension function
// that configures the client with extra options.
func WithCircuitBreaker(maxFailures int, resetTimeout time.Duration) ClientOption {
	return func(c *Client) {
		c.Client.Transport = NewCircuitBreaker(
			maxFailures,
			resetTimeout,
			c.Client.Transport,
		)
	}
}

// newClient creates a new client based on the options passed in as parameters.
func NewClient(opts ...ClientOption) *Client {
	c := &Client{
		Client: &http.Client{
			Transport: apmhttp.WrapRoundTripper(http.DefaultTransport),
			Timeout:   10 * time.Second,
		},
	}

	for _, opt := range opts {
		opt(c)
	}

	return c
}
