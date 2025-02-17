package api

import (
	"encoding/json"
	"fmt"
	"net/http"

	"github.com/popcorn-market/financial-times/internal/domain/article"
	"github.com/popcorn-market/financial-times/internal/application"
)

// ArticleHandler handles HTTP requests for articles
type ArticleHandler struct {
	articleService *service.ArticleService
}

// NewArticleHandler initializes the handler with a service
func NewArticleHandler(articleService *service.ArticleService) *ArticleHandler {
	return &ArticleHandler{articleService: articleService}
}

// CreateCompanyArticle handles the HTTP request to create a company article
func (h *ArticleHandler) CreateCompanyArticle(w http.ResponseWriter, r *http.Request) {
	// Ensure content type
	if r.Header.Get("Content-Type") != "application/json" {
		http.Error(w, "Content-Type must be application/json", http.StatusUnsupportedMediaType)
		return
	}

	// Decode JSON body
	var requestPayload article.CompanyArticle
	if err := json.NewDecoder(r.Body).Decode(&requestPayload); err != nil {
		http.Error(w, fmt.Sprintf("Failed to parse JSON: %v", err), http.StatusBadRequest)
		return
	}
	defer r.Body.Close()

	// Pass to the service
	if err := h.articleService.CreateCompanyArticle(r.Context(), requestPayload); err != nil {
		http.Error(w, fmt.Sprintf("Failed to create article: %v", err), http.StatusInternalServerError)
		return
	}

	// Success response
	w.WriteHeader(http.StatusCreated)
	fmt.Fprintf(w, "✅ Company article for '%s' created successfully!", requestPayload.Company)
}

func (h *ArticleHandler) GetCompanyArticlesByTicker(w http.ResponseWriter, r *http.Request) {
	t := r.PathValue("ticker")

	if t == ""{
		http.Error(
			w, 
			"Ticker is required",
			http.StatusNotFound,
		)
	}

	a, err := h.articleService.GetCompanyArticleByTicker(r.Context(), t)
	if err != nil {
		http.Error(
			w, 
			fmt.Sprintf("Failed to retrieve article: %v", err),
			http.StatusNotFound,
		)
		return
	}

	rd, err := json.Marshal(a)

	if(err != nil) {
		http.Error(w, err.Error(), http.StatusInternalServerError)
	}

	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(http.StatusOK)
	w.Write(rd)
}