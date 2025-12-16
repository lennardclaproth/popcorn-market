package marketdata

import (
	"context"
	"time"

	"github.com/lennardclaproth/ansar-broker/errorx"
	"github.com/lennardclaproth/ansar-broker/logging"
)

type Worker struct {
	hub      *Hub
	handler  TickerHandler
	interval time.Duration
	Out      chan<- Ticker
	log      logging.Logger
}

func (w *Worker) Run(ctx context.Context) {
	t := time.NewTicker(w.interval)
	defer t.Stop()

	for {
		select {
		case <-ctx.Done():
			return
		case <-t.C:
			w.refreshCache(ctx)
		}
	}
}

func (w *Worker) refreshCache(ctx context.Context) {
	ts := w.hub.DrainDirty()
	if len(ts) == 0 {
		return
	}

	for _, dt := range ts {
		t, err := w.handler.Fetch(ctx, dt)
		if err != nil {
			w.log.Error(ctx, "refreshCache: an error occurred while handling a request", errorx.Trace(err))
			continue
		}

		w.hub.SetTicker(t)

		w.publishToChannel(ctx, t)
	}
}

func (w *Worker) publishToChannel(ctx context.Context, t Ticker) {
	select {
	case w.Out <- t:
	case <-ctx.Done():
		return
	}
}
