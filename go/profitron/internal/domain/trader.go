package domain

type TraderStatus string
type TraderType string

const (
	TraderTypeScalper     TraderType = "scalper"
	TraderTypeSwing       TraderType = "swing"
	TraderTypePosition    TraderType = "position"
	TraderTypeFundamental TraderType = "fundamental"
)

const (
	TraderStatusIdle      TraderStatus = "idle"
	TraderStatusFetching  TraderStatus = "fetching"
	TraderStatusAnalyzing TraderStatus = "analyzing"
	TraderStatusTrading   TraderStatus = "trading"
	TraderStatusError     TraderStatus = "error"
)

type TraderConfig struct {
	TraderType TraderType
	BuyBias    float64
	SellBias   float64
}

type Trader struct {
	ID      string
	Name    string
	Status  TraderStatus
	Config  TraderConfig
}

func NewTrader(id, name string, config TraderConfig) *Trader {
	return &Trader{
		ID:     id,
		Name:   name,
		Status: TraderStatusIdle,
		Config: config,
	}
}