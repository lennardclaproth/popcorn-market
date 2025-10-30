package trader

import (
	"context"
	"crypto/rand"
	"encoding/binary"
	"fmt"
	mrand "math/rand"
	"time"

	"github.com/google/uuid"
	"github.com/lennardclaproth/profitron/errorx"
	"github.com/lennardclaproth/profitron/internal/broker"
)

type TraderStatus int
type TraderType int

const (
	TraderTypeScalper TraderType = iota
	TraderTypeSwing
	TraderTypePosition
	TraderTypeFundamental
)

const (
	TraderStatusIdle TraderStatus = iota
	TraderStatusFetching
	TraderStatusAnalyzing
	TraderStatusTrading
	TraderStatusError
)

type TraderConfig struct {
	TraderType TraderType `bson:"trader_type"`
	BuyBias    float64    `bson:"buy_bias"`
	SellBias   float64    `bson:"sell_bias"`
}

type Trader struct {
	ID       string       `bson:"trader_id"`
	Email    string       `bson:"email"`
	Password string       `bson:"password"`
	Name     string       `bson:"name"`
	Status   TraderStatus `bson:"status"`
	Config   TraderConfig `bson:"config"`
	UserID   uuid.UUID    `bson:"user_id"`
	broker   *broker.Broker
}

var (
	// Create a single global RNG instance seeded with a mix of time and crypto entropy.
	rng          = mrand.New(mrand.NewSource(seedFromCrypto()))
	emailDomains = []string{
		"gmail.com", "yahoo.com", "outlook.com", "profitron.ai", "trader.net",
	}
)

func NewTrader(id, name string, config TraderConfig) *Trader {
	return &Trader{
		ID:       id,
		Name:     name,
		Email:    randomEmail(name),
		Status:   TraderStatusIdle,
		Config:   config,
		Password: randomPassword(),
	}
}

func (t *Trader) Load(br *broker.Broker) {
	t.broker = br
}

func (t *Trader) Trade() {
	// t.broker.PlaceOrder()
}

// seedFromCrypto generates a non-deterministic int64 seed.
func seedFromCrypto() int64 {
	var b [8]byte
	if _, err := rand.Read(b[:]); err != nil {
		// fallback to time-based seed if crypto/rand fails
		return time.Now().UnixNano()
	}
	return int64(binary.LittleEndian.Uint64(b[:]))
}

func randomEmail(name string) string {
	user := fmt.Sprintf("%s%d", sanitizeName(name), rng.Intn(10000))
	domain := emailDomains[rng.Intn(len(emailDomains))]
	return fmt.Sprintf("%s@%s", user, domain)
}

// OnBoard creates a user and an account for this trader via the broker API.
// It returns the created user ID or an error.
func (t *Trader) OnBoard(ctx context.Context) (uuid.UUID, error) {
	if t.broker == nil {
		return uuid.Nil, fmt.Errorf("broker not initialized; call Load() first")
	}

	// 1. Create a user on the broker
	createReq := broker.CreateUserRequest{
		DateOfBirth: "1990-01-01", // or generate based on your simulation
		Email:       t.Email,
		FirstName:   t.Name,
		LastName:    "Trader",
		Street:      "1 Infinite Loop",
		City:        "Frankfurt",
		State:       "HE",
		ZipCode:     "60311",
		Country:     "Germany",
		Password:    t.Password,
	}

	userID, err := t.broker.CreateUser(ctx, createReq)
	if err != nil {
		return uuid.Nil, errorx.Trace(fmt.Errorf("failed to create user for trader %s: %w", t.ID, err))
	}
	t.UserID = userID

	// 2. Open an account for the created user
	initialBalance := 10_000.0 + rng.Float64()*90_000.0 // random 10k–100k
	_, err = t.broker.OpenAccount(ctx, userID, initialBalance)
	if err != nil {
		return userID, fmt.Errorf("failed to open account for user %s: %w", userID, err)
	}

	// 3. Update state
	t.Status = TraderStatusIdle
	return userID, nil
}

func sanitizeName(name string) string {
	result := make([]rune, 0, len(name))
	for _, r := range name {
		if (r >= 'a' && r <= 'z') || (r >= 'A' && r <= 'Z') {
			result = append(result, r)
		}
	}
	if len(result) == 0 {
		return "user"
	}
	return string(result)
}

func randomPassword() string {
	const letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
	b := make([]byte, 10)
	for i := range b {
		b[i] = letters[rng.Intn(len(letters))]
	}
	return string(b)
}
