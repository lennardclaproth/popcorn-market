import datetime
from models.financial_times_models import CompanyArticle, PoliticalArticle, MacroArticle, SectorArticle
from models.financial_atlas_models import FinancialStatement
from generators import sector_article_generator

def generate_company_article(
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

    article_data = generate_article(prompt)

    return CompanyArticle(
        ticker=financials.ticker_symbol,
        industry=financials.income_statement.industry,
        company_name=financials.ticker_symbol,
        date=datetime.datetime.now(),
        type="Company",
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        metadata={"reasoning": article_data["reasoning"]}
    )

def generate_sector_article(
    sector_articles: list[SectorArticle], 
    macro_economic_trends: list[MacroArticle], 
    political_landscape: list[PoliticalArticle]
) -> SectorArticle:
    """
    Generates a sector analysis article using industry trends, economic influences, and politics.
    """
    return sector_article_generator.generate(sector_articles, macro_economic_trends, political_landscape)

def generate_political_article(current_political_landscape:list):
    """
    Generates an article about the current political landscape
    based on the previous article(s)
    """
    pass

def generate_macro_article(macro_economical_trends:list, current_political_landscape:list):
    """
    Generates a macro economic article based on the previous macro
    economic article and the last political articles.
    """
    pass

def generate_financial_report():
    """
    Generates a financial report based on the previous financials and 
    the articles.
    """
    pass

def generate_company_profile(sector:str, sector_articles:list, current_political_landscape:list, macro_economical_trends:list):
    """
    Generates a company profile based on some information.
    """
    pass