from models.financial_atlas_models import Company, FinancialStatement

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
