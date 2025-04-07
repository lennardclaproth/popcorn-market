package article

// ArticleType defines the types of articles
type ArticleType string

const (
	Company       ArticleType = "company"
	Sector        ArticleType = "sector"
	MacroEconomic ArticleType = "macro-economic"
	Political     ArticleType = "political"
)

// Metadata holds additional information about the article
type Metadata struct {
	Reasoning string            `json:"reasoning" bson:"reasoning"`
	Prompt string
	Extras    map[string]string `json:"extras,omitempty" bson:"extras,omitempty"`
}
