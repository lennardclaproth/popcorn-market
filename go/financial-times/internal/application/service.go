package service

import (
	"context"
	"fmt"

	"github.com/popcorn-market/financial-times/internal/domain/article"
)

type ArticleService struct {
	repo article.Repository
}

func NewArticleService(repo article.Repository) *ArticleService {
	return &ArticleService{repo: repo}
}

func (s *ArticleService) CreateCompanyArticle(ctx context.Context, a article.CompanyArticle) error {
	if a.Ticker == "" || a.Company == "" || a.Content == "" || a.Headline == "" {
		return fmt.Errorf("missing required fields")
	}
	return s.CreateCompanyArticle(ctx, a)
}

func (s *ArticleService) GetCompanyArticleByTicker(ctx context.Context, ticker string) ([]*article.CompanyArticle, error) {
	return s.repo.ByTicker(ctx, ticker)
}
