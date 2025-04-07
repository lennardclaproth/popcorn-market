from models.financial_times_models import CompanyArticle, PoliticalArticle, MacroArticle, SectorArticle
import requests
from config import (
    COMPANY_ARTICLES_ENDPOINT,
    POLITICAL_ARTICLES_ENDPOINT,
    MACRO_ARTICLES_ENDPOINT,
    SECTOR_ARTICLES_ENDPOINT,
    PUBLISH_ARTICLE_ENDPOINT
)

def fetch_company_articles(ticker: str):
    """Fetches the last two articles of a company based on its ticker."""
    url = f"{COMPANY_ARTICLES_ENDPOINT}/{ticker}?limit=2"
    response = requests.get(url)

    if response.status_code != 200:
        raise Exception(f"Failed to fetch company articles: {response.status_code}")

    articles = response.json()
    return [CompanyArticle(**article) for article in articles]


def fetch_political_articles():
    """Fetches the last political article created."""
    url = f"{POLITICAL_ARTICLES_ENDPOINT}?limit=1"
    response = requests.get(url)

    if response.status_code != 200:
        raise Exception(f"Failed to fetch political articles: {response.status_code}")

    articles = response.json()
    return PoliticalArticle(**articles[0]) if articles else None


def fetch_macro_articles():
    """Fetches the last article about the macroeconomic environment."""
    url = f"{MACRO_ARTICLES_ENDPOINT}?limit=1"
    response = requests.get(url)

    if response.status_code != 200:
        raise Exception(f"Failed to fetch macro articles: {response.status_code}")

    articles = response.json()
    return MacroArticle(**articles[0]) if articles else None


def fetch_sector_articles(sector: str):
    """Fetches the last article about trends in a specific sector."""
    url = f"{SECTOR_ARTICLES_ENDPOINT}/{sector}?limit=1"
    response = requests.get(url)

    if response.status_code != 200:
        raise Exception(f"Failed to fetch sector articles: {response.status_code}")

    articles = response.json()
    return SectorArticle(**articles[0]) if articles else None

def publish_article(article):
    url = f"{PUBLISH_ARTICLE_ENDPOINT}"
    response = requests.post(url, json=article.model_dump(mode="json"), verify=False)   

    if response.status_code != 201:
        raise Exception(f"Failed to publish article: {response.status_code} - {response.text}")
    