package article

import "time"

// MacroArticle represents an article about macroeconomic trends
type MacroArticle struct {
	Region string `json:"region" bson:"region"`
	articleBase
}

// NewMacroArticle creates a new MacroArticle
func NewMacroArticle(region, content, headline, reasoning string) MacroArticle {
	return MacroArticle{
		Region: region,
		articleBase: articleBase{
			Date:     time.Now(),
			Type:     MacroEconomic,
			Headline: headline,
			Content:  content,
			Metadata: Metadata{Reasoning: reasoning},
		},
	}
}
