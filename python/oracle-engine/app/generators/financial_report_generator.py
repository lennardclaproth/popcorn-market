from models.financial_atlas import FinancialStatement, Company
import datetime
from services.chat import execute_prompt

def generate(company: Company, year: int, quarter: int) -> FinancialStatement:
    """
    Uses AI to generate a realistic quarterly financial report for a company.
    """

    prompt = f"""
    Generate a realistic financial report for **{company.name} (Ticker: {company.ticker})** 
    for **Q{quarter} {year}** based on industry trends.

    Ensure the numbers are reasonable for a company in the **{company.industry}** sector.

    Output as valid JSON:
    ```json
    {{
        "revenue": "float (in billions)",
        "net_income": "float (in billions)",
        "eps": "float",
        "free_cash_flow": "float (in billions)",
        "total_assets": "float (in billions)",
        "total_equity": "float (in billions)",
        "debt_to_equity_ratio": "float"
    }}
    ```
    """

    financial_data = execute_prompt(prompt)

    return FinancialStatement(
        ticker_symbol=company.ticker,
        year=year,
        interval=f"Q{quarter}",
        income_statement={
            "revenue": financial_data["revenue"],
            "net_income": financial_data["net_income"],
            "eps": financial_data["eps"],
            "free_cash_flow": financial_data["free_cash_flow"]
        },
        balance_sheet={
            "total_assets": financial_data["total_assets"],
            "total_equity": financial_data["total_equity"],
            "debt_to_equity_ratio": financial_data["debt_to_equity_ratio"]
        },
        cash_flow_statement={
            "cash_flow": financial_data["free_cash_flow"]
        }
    )
    