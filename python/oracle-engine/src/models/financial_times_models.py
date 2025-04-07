from pydantic import BaseModel, Field
from datetime import datetime
from typing import Dict, Optional


class ArticleBase(BaseModel):
    """Base model for all articles."""
    date: datetime
    type: int  # Assuming ArticleType is a string representation
    headline: str
    content: str
    metadata: Optional[Dict[str, str]] = Field(default_factory=dict)


class CompanyArticle(ArticleBase):
    """Represents an article about a specific company."""
    ticker: str
    industry: str
    company_name: str


class MacroArticle(ArticleBase):
    """Represents an article about macroeconomic trends."""
    region: str


class PoliticalArticle(ArticleBase):
    """Represents a political article."""
    region: str


class SectorArticle(ArticleBase):
    """Represents an article about trends in a specific sector."""
    sector: str
