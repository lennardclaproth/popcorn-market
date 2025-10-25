package security

import (
	"context"
	"time"

	"github.com/lennardclaproth/ansar-broker/logging"
)

type Security struct {
	Symbol string
	Name   string
}

type Listing struct {
	Symbol                string
	Isin                  string
	CompanyName           string
	LastPrice             float64
	PriceOpen             float64
	PriceClose            float64
	PriceHigh             float64
	PriceLow              float64
	PriceChange           float64
	PriceChangePercentage float64
	Volume                int
	LastUpdated           time.Time
}

type ExchangeHandler interface {
	FindListings(ctx context.Context, filter string, p int, c int) ([]Listing, error)
}

type Service struct {
	log      logging.Logger
	exchange ExchangeHandler
}

type GetListingsQuery struct {
	Filter string
	Page   int
	Count  int
}

func NewService(ex ExchangeHandler, log logging.Logger) *Service {
	return &Service{
		log:      log,
		exchange: ex,
	}
}

func (s *Service) SearchSecurities(ctx context.Context, q GetListingsQuery) ([]Listing, error) {
	listings, err := s.exchange.FindListings(ctx, q.Filter, q.Page, q.Count)
	if err != nil {
		return nil, err
	}
	return listings, nil
}
