package http

import (
	"net/http"
	"time"

	"github.com/lennardclaproth/ansar-broker/httpx"
	"github.com/lennardclaproth/ansar-broker/logging"
)

// withRequestLogging returns a http logging middleware function.
func withRequestLogging(logger logging.Logger) func(http.Handler) http.Handler {
	return func(next http.Handler) http.Handler {
		return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
			start := time.Now()

			rw := httpx.NewResponseWriter(w)
			next.ServeHTTP(rw, r)

			duration := time.Since(start)
			logger.Info(r.Context(), "request completed",
				"method", r.Method,
				"path", r.URL.Path,
				"status", rw.StatusCode,
				"bytes", rw.Size,
				"duration_ms", duration.Milliseconds(),
			)
		})
	}
}
