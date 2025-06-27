import requests
from constants.financial_atlas import COMPANY_ENDPOINT, MARKET_DATA_ENDPOINT, TICKERS_ENDPOINT
from models.financial_atlas import Company, FinancialStatement, MarketData, MarketSnapshot

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


def fetch_company(ticker) -> Company:
    """
    Fetches a company from the financial atlas service
    based on the ticker
    """
    url = f"{COMPANY_ENDPOINT}/{ticker}"
    response = requests.get(url, verify=False)
    if response.status_code != 200 and response.status_code != 404:
        raise Exception("Failed to fetch company: %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    company = response_body.get("company", None)

    if company == None:
        raise Exception("Company is not contained in response: %s", response.text)

    return Company(**company)

def fetch_current_market_snapshot(ticker : str) -> MarketSnapshot:
    url = f"{MARKET_DATA_ENDPOINT}/{ticker}/current"
    response = requests.get(url, verify=False)
    if response.status_code != 200 and response.status_code != 404:
        raise Exception("Failed to fetch snapshot: %s - %s", response.status_code, response.text)

    try:
        response_body = response.json()
    except ValueError as e:
        raise Exception("Invalid JSON in response") from e

    snapshot = response_body.get("marketSnapshot", None)

    if snapshot == None:
        raise Exception("Snapshot is not contained in response: %s", response.text)

    return MarketSnapshot(**snapshot)


def fetch_company_financials(ticker : str) -> FinancialStatement:
    """
    Fetches the financials of a company based on the ticker
    """
    return []

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
    