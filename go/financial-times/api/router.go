package api

import (
	"net/http"
)

// SetupRouter sets up the HTTP routes using Go 1.22's new ServeMux pattern
func SetupRouter(h *ArticleHandler) *http.ServeMux {
	mux := http.NewServeMux()

	// Correct pattern: "METHOD /path"
	mux.HandleFunc("POST /api/v1/article/company", h.CreateCompanyArticle)
	mux.HandleFunc("GET /api/v1/article/company/{ticker}", h.GetCompanyArticlesByTicker)

	return mux
}
