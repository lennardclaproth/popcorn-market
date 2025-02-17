package trader

import "context"

type TraderType string

const (
	Aggressive   TraderType = "Aggressive"
	Conservative TraderType = "Conservative"
	Neutral      TraderType = "Neutral"
)

type Trader struct {
	ID         string
	TraderType TraderType
}

func (t *Trader) Analyze(ctx context.Context) float64 {
	return 1.0
}
