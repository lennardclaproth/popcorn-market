package marketdata

import (
	"context"
	"time"
)

type Ticker struct {
	Symbol                string
	LastPrice             float64
	PriceHigh             float64
	PriceLow              float64
	PriceChange           float64
	PriceChangePercentage float64
	Volume                int
	LastUpdated           time.Time
}

type TickerHandler interface {
	Fetch(ctx context.Context, sym string) (Ticker, error)
}
