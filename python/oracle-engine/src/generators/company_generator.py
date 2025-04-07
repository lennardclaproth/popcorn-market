from models.financial_atlas_models import Company
from ai.llm_client import execute_prompt

def generate(ticker: str) -> Company:
    """
    Uses AI to generate a structured company profile.
    """
    
    prompt = f"""
    Generate a realistic company profile for the ticker **{ticker}**.
    
    Output the result in valid JSON:
    ```json
    {{
        "name": "Company Name",
        "industry": "Industry Sector",
        "description": "Short description of the company.",
        "headquarters": "City, Country",
        "ceo": "CEO Name",
        "founded_year": 2000,
        "employees": 25000
    }}
    ```
    """

    company_data = execute_prompt(prompt)

    return Company(
        ticker=ticker,
        name=company_data["name"],
        industry=company_data["industry"],
        description=company_data["description"],
        headquarters=company_data["headquarters"],
        ceo=company_data["ceo"],
        founded_year=company_data["founded_year"],
        employees=company_data["employees"]
    )
