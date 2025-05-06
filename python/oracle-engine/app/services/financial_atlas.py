import requests
from config import COMPANY_ENDPOINT, MARKET_DATA_ENDPOINT
from models.financial_atlas import Company, FinancialStatement, MarketData

def fetch_tickers() -> list[str]:
    """
    Fetches the available tickers from the FinancialAtlas
    service.
    """
    raise NotImplementedError()

def fetch_company(ticker) -> Company:
    """
    Fetches a company from the financial atlas service
    based on the ticker
    """
    raise NotImplementedError()

def fetch_company_financials(ticker : str) -> FinancialStatement:
    """
    Fetches the financials of a company based on the ticker
    """
    raise NotImplementedError()

def create_company(company_profile : Company):
    """
    Creates a new company by posting a company to the server
    """
    url = f"{COMPANY_ENDPOINT}"
    json = company_profile.model_dump(mode="json")
    response = requests.post(url, json=json, verify=False)

    if response.status_code != 201 and response.status_code != 404:
        raise Exception("Failed to create company: %s - %s", response.status_code, response.text)
    
def publish_market_data(market_data: MarketData):
    """
    Publishes market data of a company
    """
    url = f"{MARKET_DATA_ENDPOINT}/{market_data.ticker}"
    json=market_data.model_dump(mode="json")
    response = requests.post(url, json=json, verify=False)
    
    if response.status_code != 201 and response.status_code != 404:
        raise Exception("Failed to publish market data: %s - %s", response.status_code, response.text)
    