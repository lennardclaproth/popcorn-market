from models.financial_times import CompanyArticle, PoliticalArticle, MacroArticle, SectorArticle
import requests
from constants.financial_times import (
    COMPANY_ARTICLES_ENDPOINT,
    POLITICAL_ARTICLES_ENDPOINT,
    MACRO_ARTICLES_ENDPOINT,
    SECTOR_ARTICLES_ENDPOINT,
    PUBLISH_ARTICLE_ENDPOINT,
    TICKERS_ENDPOINT
)

ARTICLE_LIMIT = 3

def fetch_tickers() -> list[str]:
    """
    Fetches the available tickers from the FinancialAtlas
    service.
    """
    url = f"{TICKERS_ENDPOINT}"
    response = requests.get(url, verify=False)

    if response.status_code == 404:
        return []

    if response.status_code != 200 and response.status_code != 404:
        raise Exception("Failed to fetch tickers: %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    tickers = response_body.get("tickers", [])
    return tickers


def fetch_company_articles(ticker: str):
    """Fetches the last two articles of a company based on its ticker."""
    url = f"{COMPANY_ARTICLES_ENDPOINT}/{ticker}?limit={ARTICLE_LIMIT}"
    response = requests.get(url, verify=False)

    if response.status_code == 404:
        return []

    if response.status_code != 200:
        raise Exception("Failed to fetch company articles:  %s - %s", response.status_code, response.text)
    
    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    articles = response_body.get("articles", [])
    
    return [CompanyArticle(**article) for article in articles]


def fetch_political_articles():
    """Fetches the last political article created."""
    url = f"{POLITICAL_ARTICLES_ENDPOINT}?limit={ARTICLE_LIMIT}"
    response = requests.get(url, verify=False)

    if response.status_code == 404:
        return []

    if response.status_code != 200:
        raise Exception("Failed to fetch political articles: %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    articles = response_body.get("articles", [])
    return [PoliticalArticle(**article) for article in articles]

def fetch_political_articles_by_region(region):
    """Fetches the last political article created."""
    url = f"{POLITICAL_ARTICLES_ENDPOINT}/region/{region}?limit={ARTICLE_LIMIT}"
    response = requests.get(url, verify=False)

    if response.status_code == 404:
        return []

    if response.status_code != 200:
        raise Exception("Failed to fetch political articles: %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    articles = response_body.get("articles", [])
    return [PoliticalArticle(**article) for article in articles]


def fetch_macro_articles():
    """Fetches the last article about the macroeconomic environment."""
    url = f"{MACRO_ARTICLES_ENDPOINT}?limit={ARTICLE_LIMIT}"
    response = requests.get(url, verify=False)

    if response.status_code == 404:
        return []

    if response.status_code != 200:
        raise Exception("Failed to fetch macro articles:  %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    articles = response_body.get("articles", [])
    return [MacroArticle(**article) for article in articles]

def fetch_macro_articles_by_region(region):
    """Fetches the last article about the macroeconomic environment."""
    url = f"{MACRO_ARTICLES_ENDPOINT}/region/{region}?limit={ARTICLE_LIMIT}"
    response = requests.get(url, verify=False)

    if response.status_code == 404:
        return []

    if response.status_code != 200:
        raise Exception("Failed to fetch macro articles:  %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    articles = response_body.get("articles", [])
    return [MacroArticle(**article) for article in articles]


def fetch_sector_articles(sector: str):
    """Fetches the last article about trends in a specific sector."""
    url = f"{SECTOR_ARTICLES_ENDPOINT}/{sector}?limit={ARTICLE_LIMIT}"
    response = requests.get(url, verify=False)

    if response.status_code == 404:
        return []

    if response.status_code != 200 and response.status_code != 404:
        raise Exception("Failed to fetch sector articles: %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    articles = response_body.get("articles", [])
    return [SectorArticle(**article) for article in articles]

def publish_article(article) -> str:
    url = f"{PUBLISH_ARTICLE_ENDPOINT}"
    json = article.model_dump(mode="json")
    response = requests.post(url, json=json, verify=False)   

    if response.status_code != 201 and response.status_code != 404:
        raise Exception("Failed to publish article: %s - %s", response.status_code, response.text)
    
    if response.content != None:
        data = response.json()
        return data["id"]
    