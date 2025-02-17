package article

import (
	"context"
)

type Repository interface {
	ByTicker(ctx context.Context, t string) ([]*CompanyArticle, error)
	Create(ctx context.Context, a Article) error
}
