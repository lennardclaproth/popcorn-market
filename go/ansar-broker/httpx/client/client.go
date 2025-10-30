package httpx

import (
	"net/http"
	"time"
)

type Client struct {
	*http.Client
}

type ClientOption func(*Client)

func WithCircuitBreaker(maxFailures int, resetTimeout time.Duration) ClientOption {
	return func(c *Client) {
		c.Client.Transport = NewCircuitBreaker(
			maxFailures,
			resetTimeout,
			c.Client.Transport,
		)
	}
}

func NewClient(opts ...ClientOption) *Client {
	c := &Client{
		Client: &http.Client{
			Transport: http.DefaultTransport,
			Timeout:   10 * time.Second,
		},
	}

	for _, opt := range opts {
		opt(c)
	}

	return c
}
