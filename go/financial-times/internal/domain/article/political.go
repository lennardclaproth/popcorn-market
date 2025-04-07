package article

import "time"

// GlobalArticle represents a global political news article
type PoliticalArticle struct {
	Region string `json:"region" bson:"region"`
	articleBase
}

// NewGlobalArticle creates a new GlobalArticle
func NewPoliticalArticle(region, content, headline, reasoning string, metadata map[string]string) PoliticalArticle {
	return PoliticalArticle{
		Region: region,
		articleBase: articleBase{
			Date:     time.Now(),
			Type:     Political,
			Headline: headline,
			Content:  content,
			Metadata: metadata,
		},
	}
}
