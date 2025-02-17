package article

import "time"

// SectorArticle represents an article about a specific sector
type SectorArticle struct {
	Sector string `json:"sector" bson:"sector"`
	articleBase
}

// NewSectorArticle creates a new SectorArticle
func NewSectorArticle(sector, content, headline, reasoning string) SectorArticle {
	return SectorArticle{
		Sector: sector,
		articleBase: articleBase{
			Date:     time.Now(),
			Type:     Sector,
			Headline: headline,
			Content:  content,
			Metadata: Metadata{Reasoning: reasoning},
		},
	}
}
