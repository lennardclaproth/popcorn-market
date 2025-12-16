package marketdata

import (
	"errors"
	"fmt"
	"sync"
)

type Hub struct {
	mux        sync.RWMutex
	// secs -> client
	secSubs    map[string]map[string]struct{}
	// clients -> secs
	clientSubs map[string]map[string]struct{}
	tickers    map[string]Ticker
	dirty      map[string]Ticker
}

func NewHub() *Hub {
	return &Hub{
		mux:        sync.RWMutex{},
		secSubs:    make(map[string]map[string]struct{}),
		clientSubs: make(map[string]map[string]struct{}),
		tickers:    make(map[string]Ticker),
		dirty:      make(map[string]Ticker),
	}
}

var (
	ErrUnknownClient         = errors.New("marketdata: unknown client")
	ErrUnknownSecurity       = errors.New("marketdata: unknown security")
	ErrTickerNotCached       = errors.New("marketdata: ticker not cached")
	ErrSecurityNotSubscribed = errors.New("marketdata: security not subscribed")
)

// subscribe adds a new client with the symbol they want to subscribe to
// to the hub. It locks it and updates the secToClients and clientsToSec
// if the symbol is not cached it will be added to the tickers map and
// the data will be set.
func (h *Hub) Subscribe(c string, syms []string) {
	h.mux.Lock()
	defer h.mux.Unlock()

	for _, sym := range syms {
		// create client in client subs when client does not exist yet
		if _, ok := h.clientSubs[c]; !ok {
			h.clientSubs[c] = make(map[string]struct{})
		}
		// set symbol in map
		h.clientSubs[c][sym] = struct{}{}
		// add security to security subs when it does not exist yet
		if _, ok := h.secSubs[sym]; !ok {
			h.secSubs[sym] = make(map[string]struct{})
		}
		// set client in map
		h.secSubs[sym][c] = struct{}{}
		// check if symbol is already contained in tickers
		_, ok := h.tickers[sym]
		if ok {
			continue
		}
		// if not contained set empty ticker and mark ticker
		// ticker as dirty
		h.tickers[sym] = Ticker{}
		h.dirty[sym] = Ticker{}
	}
}

// unsubscribe removes a subscription of the a client. If there are no
// clients left on that subscription it removes the ticker from the
// cache and the dirty cache if needed.
func (h *Hub) Unsubscribe(c string, syms []string) error {
	h.mux.Lock()
	defer h.mux.Unlock()

	// Get a map from all the securities a client is mapped to
	_, ok := h.clientSubs[c]
	if !ok {
		return fmt.Errorf("unsubscribe %q: %w", c, ErrUnknownClient)
	}

	// loop over the securities of the client
	for _, sym := range syms {
		// lookup the symbol to client relation
		if clients, ok := h.secSubs[sym]; ok {
			delete(clients, c)
		}
		if secs, ok := h.clientSubs[c]; ok {
			delete(secs, sym)
		}
		h.prune(sym, c)
	}
	return nil
}

// prune checks if the symbol or the client should be pruned. Deletes it from
// the cache as well if needed.
func (h *Hub) prune(sym, c string) {
	// prune the security when no client is subscribed to the security anymore
	if clients, ok := h.secSubs[sym]; ok && len(clients) == 0 {
		delete(h.secSubs, sym)
		delete(h.tickers, sym)
		delete(h.dirty, sym)
	}
	// prune the client when the client has no subscriptions anymore
	if secs, ok := h.clientSubs[c]; ok && len(secs) == 0 {
		delete(h.clientSubs, c)
	}
}

func (h *Hub) RemoveClient(c string) {
	h.mux.Lock()
	defer h.mux.Unlock()

	secs, ok := h.clientSubs[c]
	if !ok {
		return
	}

	for client := range secs {
		// lookup the symbol to client relation
		if clients, ok := h.secSubs[client]; ok {
			delete(clients, c)
			if len(clients) != 0 {
				continue
			}
			// Make sure to remove the ticker from the list
			// and clean up when there are no clints
			h.prune(client, c)
		}
	}

	delete(h.clientSubs, c)
}

func (h *Hub) SetTicker(t Ticker) error {
	h.mux.Lock()
	defer h.mux.Unlock()

	if _, exists := h.secSubs[t.Symbol]; !exists {
		return fmt.Errorf("setTicker %q: %w", t.Symbol, ErrSecurityNotSubscribed)
	}

	h.tickers[t.Symbol] = t
	return nil
}

func (h *Hub) MarkDirty(sym string) error {
	h.mux.Lock()
	defer h.mux.Unlock()

	if _, exists := h.dirty[sym]; exists {
		return nil
	}

	t, exists := h.tickers[sym]
	if !exists {
		return fmt.Errorf("markDirty %q: %w", sym, ErrTickerNotCached)
	}

	h.dirty[sym] = t
	return nil
}

func (h *Hub) DrainDirty() []string {
	h.mux.Lock()
	defer h.mux.Unlock()

	if len(h.dirty) == 0 {
		return nil
	}

	symbols := make([]string, 0, len(h.dirty))
	for sym := range h.dirty {
		symbols = append(symbols, sym)
	}

	h.dirty = make(map[string]Ticker)
	return symbols
}
