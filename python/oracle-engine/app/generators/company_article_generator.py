from typing import Generator
from models.financial_times import CompanyArticle, PoliticalArticle, MacroArticle, SectorArticle
from models.financial_atlas import FinancialStatement
from services.chat import execute_prompt

def generate(
    sector_articles: list[SectorArticle], 
    macro_economic_trends: list[MacroArticle], 
    company_articles: list[CompanyArticle], 
    financials: FinancialStatement, 
    political_landscape: list[PoliticalArticle]
) -> CompanyArticle:
    """
    Generates a company-specific article based on financials, market data, and external factors.
    """

    prompt = f"""
    🔹 **Generate a Company Article** 🔹

    **Company Overview**
    - Name: {financials.ticker_symbol}
    - Industry: {financials.income_statement.industry}
    - Revenue: {financials.income_statement.revenue}
    - Net Income: {financials.income_statement.net_income}
    - EPS: {financials.income_statement.eps}

    **Recent Sector & Macro Trends**
    - Sector News: {sector_articles}
    - Macroeconomic Trends: {macro_economic_trends}
    - Political Landscape: {political_landscape}

    **Recent Company News**
    {company_articles}

    ### **Task:**
    Generate an article about the company’s **current financial outlook**.

    🔹 **Output Format:** Return a JSON object:
    ```json
    {{
        "headline": "A short, engaging headline",
        "article": "A concise article"
    }}
    ```
    """

    article_data = execute_prompt(prompt)

    return CompanyArticle(
        ticker=financials.ticker_symbol,
        industry=financials.income_statement.industry,
        company_name=financials.ticker_symbol,
        type=0,
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        metadata={"reasoning": article_data["reasoning"]}
    )
