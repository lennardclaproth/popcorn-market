package marketdata

import (
	"errors"
	"testing"
)

func TestSubscribe_Symbol_ToBeAdded(t *testing.T) {
	tests := []struct {
		name    string
		c       string
		symbols []string
	}{
		{
			name:    "single symbol",
			c:       "client-1",
			symbols: []string{"BABY:TEST"},
		},
		{
			name:    "multiple symbols",
			c:       "client-1",
			symbols: []string{"BABY:TEST", "BABY:DUMMY"},
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			h := NewHub()

			// Act
			h.Subscribe(tt.c, tt.symbols)

			// Assert – lock for reading to be race-detector friendly
			h.mux.RLock()
			defer h.mux.RUnlock()

			clientSyms, ok := h.clientSubs[tt.c]
			if !ok {
				t.Fatalf("client %q not in clientsToSecs", tt.c)
			}

			// We only care that each *distinct* symbol is present
			wantSyms := unique(tt.symbols)

			for _, sym := range wantSyms {
				if _, ok := clientSyms[sym]; !ok {
					t.Fatalf("expected client %q to be subscribed to %q in clientsToSecs", tt.c, sym)
				}

				secClients, ok := h.secSubs[sym]
				if !ok {
					t.Fatalf("expected symbol %q in secToClients", sym)
				}
				if _, ok := secClients[tt.c]; !ok {
					t.Fatalf("expected symbol %q to have client %q in secToClients", sym, tt.c)
				}
			}
		})
	}
}

func TestSubscribe_NonExistingSymbol_ToBeAddedToCache(t *testing.T) {
	tests := []struct {
		name    string
		c       string
		symbols []string
	}{
		{
			name:    "single symbol",
			c:       "client-1",
			symbols: []string{"BABY:TEST"},
		},
		{
			name:    "multiple symbols",
			c:       "client-1",
			symbols: []string{"BABY:TEST", "BABY:DUMMY"},
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// ARRANGE
			h := NewHub()
			client := "client-1"
			symbols := []string{"BABY:TEST"}
			// ACT
			h.Subscribe(client, symbols)
			for _, sym := range symbols {
				if _, exists := h.tickers[sym]; !exists {
					t.Fatalf("Symbol %q is not contained in cache of hub.", sym)
				}
				if _, exists := h.dirty[sym]; !exists {
					t.Fatalf("Symbol %q is not contained in dirty securities of hub.", sym)
				}
			}
		})
	}
}

func TestUnsubscribe_Symbol_ToBeRemoved(t *testing.T) {
	tests := []struct {
		name    string
		c       string
		symbols []string
	}{
		{
			name:    "single symbol",
			c:       "client-1",
			symbols: []string{"BABY:TEST"},
		},
		{
			name:    "multiple symbols",
			c:       "client-1",
			symbols: []string{"BABY:TEST", "BABY:DUMMY"},
		},
		{
			name:    "duplicate symbols",
			c:       "client-1",
			symbols: []string{"BABY:TEST", "BABY:TEST"},
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			// Arrange
			h := NewHub()
			seedHub(h)

			// Act
			h.Unsubscribe(tt.c, tt.symbols)

			// Assert – lock for reading to be race-detector friendly
			h.mux.RLock()
			defer h.mux.RUnlock()

			// check if client is in secs
			for _, sym := range tt.symbols {
				if clients, ok := h.secSubs[sym]; ok {
					if _, exists := clients[tt.c]; exists {
						t.Fatalf("expected client %q to not be contained in subscriptions for symbol %q", tt.c, sym)
					}
				}

				if secs, ok := h.clientSubs[tt.c]; ok {
					if _, exists := secs[sym]; exists {
						t.Fatalf("expected symbol %q to not be contained in subscriptions of client %q", sym, tt.c)
					}
				}
			}
		})
	}
}

func TestUnsubscribe_UnknownClient_ToReturnError(t *testing.T) {
	h := NewHub()

	err := h.Unsubscribe("ghost-client", []string{"BABY:TEST"})
	if err == nil {
		t.Fatalf("expected error when unsubscribing unknown client, got nil")
	}
	if !errors.Is(err, ErrUnknownClient) {
		t.Fatalf("expected ErrUnknownClient, got %v", err)
	}
}

func TestRemoveClient_RemovesClientAndSubscriptions(t *testing.T) {
	h := NewHub()
	seedHub(h) // BABY:TEST -> client-1, client-2; BABY:DUMMY -> client-1

	// Act
	h.RemoveClient("client-1")

	// Assert
	h.mux.RLock()
	defer h.mux.RUnlock()

	// client-1 should be gone
	if _, ok := h.clientSubs["client-1"]; !ok {
		// good
	} else {
		t.Fatalf("expected client-1 to be removed from clientSubs")
	}

	// BABY:TEST should still exist, but only with client-2
	testClients, ok := h.secSubs["BABY:TEST"]
	if !ok {
		t.Fatalf("expected BABY:TEST to still be in secSubs")
	}
	if _, exists := testClients["client-1"]; exists {
		t.Fatalf("expected client-1 to be removed from BABY:TEST subscriptions")
	}
	if _, exists := testClients["client-2"]; !exists {
		t.Fatalf("expected client-2 to still be subscribed to BABY:TEST")
	}

	// BABY:DUMMY should be fully pruned (client-1 was the only one)
	if _, ok := h.secSubs["BABY:DUMMY"]; ok {
		t.Fatalf("expected BABY:DUMMY to be pruned from secSubs")
	}
}

func TestRemoveClient_UnknownClient_NoOp(t *testing.T) {
	h := NewHub()
	seedHub(h)

	// snapshot sizes
	h.mux.RLock()
	beforeSec := len(h.secSubs)
	beforeClients := len(h.clientSubs)
	h.mux.RUnlock()

	// Act
	h.RemoveClient("ghost-client")

	// Assert
	h.mux.RLock()
	defer h.mux.RUnlock()

	if got := len(h.secSubs); got != beforeSec {
		t.Fatalf("expected secSubs size to remain %d, got %d", beforeSec, got)
	}
	if got := len(h.clientSubs); got != beforeClients {
		t.Fatalf("expected clientSubs size to remain %d, got %d", beforeClients, got)
	}
}

func TestSetTicker_SecurityNotSubscribed_ReturnsError(t *testing.T) {
	h := NewHub()

	err := h.SetTicker(Ticker{Symbol: "BABY:UNKNOWN"})
	if err == nil {
		t.Fatalf("expected error when setting ticker for unsubscribed security, got nil")
	}
	if !errors.Is(err, ErrSecurityNotSubscribed) {
		t.Fatalf("expected ErrSecurityNotSubscribed, got %v", err)
	}

	h.mux.RLock()
	defer h.mux.RUnlock()
	if _, ok := h.tickers["BABY:UNKNOWN"]; ok {
		t.Fatalf("did not expect ticker for BABY:UNKNOWN to be cached")
	}
}

func TestSetTicker_SetsTickerForSubscribedSecurity(t *testing.T) {
	h := NewHub()
	client := "client-1"
	sym := "BABY:TEST"

	// Arrange: subscribe so secSubs entry exists
	h.Subscribe(client, []string{sym})

	// Act
	tk := Ticker{Symbol: sym}
	err := h.SetTicker(tk)
	if err != nil {
		t.Fatalf("expected no error when setting ticker for subscribed security, got %v", err)
	}

	// Assert
	h.mux.RLock()
	defer h.mux.RUnlock()

	got, ok := h.tickers[sym]
	if !ok {
		t.Fatalf("expected ticker for %q to be cached", sym)
	}
	if got.Symbol != sym {
		t.Fatalf("expected cached ticker symbol %q, got %q", sym, got.Symbol)
	}
}

func TestMarkDirty_TickerCached_AddsToDirty(t *testing.T) {
	h := NewHub()

	sym := "BABY:TEST"

	// Arrange: put ticker in cache manually
	h.mux.Lock()
	h.tickers[sym] = Ticker{Symbol: sym}
	h.mux.Unlock()

	// Act
	err := h.MarkDirty(sym)
	if err != nil {
		t.Fatalf("expected no error from MarkDirty for cached ticker, got %v", err)
	}

	// Assert
	h.mux.RLock()
	defer h.mux.RUnlock()

	if _, ok := h.dirty[sym]; !ok {
		t.Fatalf("expected %q to be present in dirty map", sym)
	}
}

func TestMarkDirty_TickerAlreadyDirty_Noop(t *testing.T) {
	h := NewHub()

	sym := "BABY:TEST"

	// Arrange: ticker + already dirty
	h.mux.Lock()
	h.tickers[sym] = Ticker{Symbol: sym}
	h.dirty[sym] = Ticker{Symbol: sym}
	beforeLen := len(h.dirty)
	h.mux.Unlock()

	// Act
	err := h.MarkDirty(sym)
	if err != nil {
		t.Fatalf("expected no error from MarkDirty on already-dirty ticker, got %v", err)
	}

	// Assert
	h.mux.RLock()
	defer h.mux.RUnlock()

	if got := len(h.dirty); got != beforeLen {
		t.Fatalf("expected dirty length to stay %d, got %d", beforeLen, got)
	}
}

func TestMarkDirty_TickerNotCached_ReturnsError(t *testing.T) {
	h := NewHub()

	sym := "BABY:TEST"

	err := h.MarkDirty(sym)
	if err == nil {
		t.Fatalf("expected error when marking non-cached ticker dirty, got nil")
	}
	if !errors.Is(err, ErrTickerNotCached) {
		t.Fatalf("expected ErrTickerNotCached, got %v", err)
	}

	h.mux.RLock()
	defer h.mux.RUnlock()
	if _, ok := h.dirty[sym]; ok {
		t.Fatalf("did not expect %q to be in dirty after error", sym)
	}
}

func TestDrainDirty_Empty_ReturnsNil(t *testing.T) {
	h := NewHub()

	syms := h.DrainDirty()
	if syms != nil && len(syms) != 0 {
		t.Fatalf("expected nil or empty slice when no dirty symbols, got %v", syms)
	}

	h.mux.RLock()
	defer h.mux.RUnlock()
	if len(h.dirty) != 0 {
		t.Fatalf("expected dirty map to remain empty, got len=%d", len(h.dirty))
	}
}

func TestDrainDirty_ReturnsAllAndClears(t *testing.T) {
	h := NewHub()

	// Arrange: mark some dirty entries directly
	h.mux.Lock()
	h.dirty["BABY:TEST"] = Ticker{Symbol: "BABY:TEST"}
	h.dirty["BABY:DUMMY"] = Ticker{Symbol: "BABY:DUMMY"}
	h.mux.Unlock()

	// Act
	syms := h.DrainDirty()

	// Assert: got 2 symbols (order unspecified)
	if len(syms) != 2 {
		t.Fatalf("expected 2 dirty symbols, got %d (%v)", len(syms), syms)
	}

	gotSet := make(map[string]struct{}, len(syms))
	for _, s := range syms {
		gotSet[s] = struct{}{}
	}

	if _, ok := gotSet["BABY:TEST"]; !ok {
		t.Fatalf("expected BABY:TEST in drained symbols, got %v", syms)
	}
	if _, ok := gotSet["BABY:DUMMY"]; !ok {
		t.Fatalf("expected BABY:DUMMY in drained symbols, got %v", syms)
	}

	// dirty should be cleared
	h.mux.RLock()
	defer h.mux.RUnlock()
	if len(h.dirty) != 0 {
		t.Fatalf("expected dirty map to be cleared after drain, got len=%d", len(h.dirty))
	}
}

// helper: deduplicate symbols
func unique(ss []string) []string {
	m := make(map[string]struct{}, len(ss))
	for _, s := range ss {
		m[s] = struct{}{}
	}
	out := make([]string, 0, len(m))
	for s := range m {
		out = append(out, s)
	}
	return out
}

func seedHub(h *Hub) {
	h.secSubs["BABY:TEST"] = map[string]struct{}{
		"client-1": {},
		"client-2": {},
	}
	h.secSubs["BABY:DUMMY"] = map[string]struct{}{
		"client-1": {},
	}
	h.clientSubs["client-1"] = map[string]struct{}{
		"BABY:TEST":  {},
		"BABY:DUMMY": {},
	}
	h.clientSubs["client-2"] = map[string]struct{}{
		"BABY:TEST": {},
	}
	h.tickers["BABY:TEST"] = Ticker{Symbol: "BABY:TEST"}
}
