package httpx

import (
	"errors"
	"net/http"
	"sync"
	"time"

	"github.com/lennardclaproth/ansar-broker/errorx"
)

type State int

const (
	Closed State = iota
	Open
	HalfOpen
)

type CircuitBreaker struct {
	next            http.RoundTripper
	mu              sync.Mutex
	state           State
	failures        int
	lastFailure     time.Time
	maxFailures     int
	resetTimeout    time.Duration
	halfOpenTrials  int
	successesInHalf int
	trialsInHalf    int
}

var (
	ErrOpen = errors.New("circuit breaker is open")
)

func NewCircuitBreaker(maxFailures int, resetTimeout time.Duration, next http.RoundTripper) *CircuitBreaker {
	if next == nil {
		next = http.DefaultTransport
	}
	return &CircuitBreaker{
		next:         next,
		state:        Closed,
		maxFailures:  maxFailures,
		resetTimeout: resetTimeout,
	}
}

// roundtrip is a function of the net/http package. it acts as a sort of middleware for
// request response wrapping. We use this function to wrap the circuit breaker around
// the request logic of the client so that we can nicely implement this circuit breaker
// with the client.
func (cb *CircuitBreaker) RoundTrip(req *http.Request) (*http.Response, error) {
	var resp *http.Response
	err := cb.execute(func() error {
		var e error
		resp, e = cb.next.RoundTrip(req)
		return e
	})

	if errors.Is(err, ErrOpen) {
		return nil, err
	}
	return resp, err
}

// execute calls the function passed in as a variable. It uses
// the circuit breaker pattern to either fail fast or continue
// through the flow
func (cb *CircuitBreaker) execute(fn func() error) error {
	// set a write lock on the cb
	cb.mu.Lock()
	switch cb.state {
	// if the circuit breaker is open we check if the resetTimeout
	// is passed. if the reset timeout it passed we change the state
	// to half open. if the reset timeout is not passed we we return
	// a CircuitOpenError
	case Open:
		if time.Since(cb.lastFailure) > cb.resetTimeout {
			// we lock so that we can be sure that the write here
			// is atomic
			cb.state = HalfOpen
		} else {
			// unlock the mutex
			cb.mu.Unlock()
			return errorx.Trace(ErrOpen)
		}
	}
	// unlock the mutex, we come here when we switch to state half open
	cb.mu.Unlock()
	// exucute the function
	err := fn()
	// lock the mutex to change the data of the circuit breaker
	cb.mu.Lock()
	// make sure we always unlock the mutex
	defer cb.mu.Unlock()

	switch cb.state {
	case Closed:
		// when the circuit breaker is closed and an error occurs
		// we close it immediately.
		if err != nil {
			cb.failures++
			if cb.failures >= cb.maxFailures {
				cb.trip()
				break
			}
			cb.failures = 0
		}
	case HalfOpen:
		// when the circuit breaker is halfopen and an error occurs
		// we close it immediately again.
		cb.trialsInHalf++
		if err != nil {
			cb.trip()
			break
		}
		// when the request successfully passes we check increase
		// the success count and check if it bigger then the threshold
		// for for the trials. when it is bigger we reset the circuit
		// breaker closing it again.
		cb.successesInHalf++
		if cb.successesInHalf >= cb.halfOpenTrials {
			cb.reset()
		}
	}

	return err
}

func (cb *CircuitBreaker) trip() {
	cb.state = Open
	cb.failures = 0
	cb.lastFailure = time.Now()
}

func (cb *CircuitBreaker) reset() {
	cb.state = Closed
	cb.failures = 0
}
