package article

import "time"

// CompanyArticle represents an article about a specific company
type CompanyArticle struct {
	Ticker   string `json:"ticker" bson:"ticker"`
	Industry string `json:"industry" bson:"industry"`
	Company  string `json:"company_name" bson:"company_name"`
	articleBase
}

// NewCompanyArticle creates a new CompanyArticle
func NewCompanyArticle(ticker, industry, company, content, headline string, metadata map[string] string) CompanyArticle {
	return CompanyArticle{
		Ticker:   ticker,
		Industry: industry,
		Company:  company,
		articleBase: articleBase{
			Date:     time.Now(),
			Type:     Company,
			Headline: headline,
			Content:  content,
			Metadata: metadata,
		},
	}
}
