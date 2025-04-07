package article

import (
	"time"
)

// Article defines the common interface for all articles
type Article interface {
	GetType() ArticleType
	GetDate() time.Time
	GetHeadline() string
	GetContent() string
	GetMetadata() Metadata
}

// articleBase holds common fields for all articles
type articleBase struct {
	Date     time.Time   `json:"date" bson:"date"`
	Type     ArticleType `json:"type" bson:"type"`
	Headline string      `json:"headline" bson:"headline"`
	Content  string      `json:"content" bson:"content"`
	Metadata map[string]string `json:"metadata,omitempty" bson:"metadata,omitempty"`
}

// Implement Article interface for articleBase
func (a articleBase) GetType() ArticleType { return a.Type }
func (a articleBase) GetDate() time.Time   { return a.Date }
func (a articleBase) GetHeadline() string  { return a.Headline }
func (a articleBase) GetContent() string   { return a.Content }
func (a articleBase) GetMetadata() map[string]string { return a.Metadata }
