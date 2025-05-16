import logging
import random
from constants.prompt import REGIONS
from core import article_formatter
from models.financial_times import CompanyArticle, SectorArticle, PoliticalArticle, MacroArticle
from models.generator import Generator
from services.chat import execute_prompt
from services import financial_times
from constants import COMPANY_ANGLES, TICKERS

logger = logging.getLogger("worker_app")

generator = Generator(
    active=True,
    description="Generates a company article",
    representation="company_article_generator",
    name="Company article generator",
    probability=1.0
)

REPRESENTATION = generator.representation

COMPANY_ANGLES = [
    "Highlight how earnings reflect resilience amid sector pressure",
    "Focus on strategic investments driving growth or concern",
    "Analyze recent financial report implications",
    "Evaluate management decisions and public perception",
    "Explore how external factors are influencing investor sentiment",
    "Report on innovation, product launches, or regulatory pressure",
    "Assess company response to macroeconomic conditions",
    "Compare the companys trajectory to sector competitors"
]

def __build_prompt(
    company_name: str,
    ticker: str,
    industry: str,
    financials: dict,
    company_articles: list[CompanyArticle],
    sector_articles: list[SectorArticle],
    macro_trends: list[MacroArticle],
    political_trends: list[PoliticalArticle]
) -> str:
    angle = random.choice(COMPANY_ANGLES)

    company_news = article_formatter.format(company_articles)
    sector_news = article_formatter.format(sector_articles)
    macro_news = article_formatter.format(macro_trends)
    political_news = article_formatter.format(political_trends)

    return f"""
🔹 **Generate a Company Article** 🔹

🌍 **Regional Focus**: {region}

🏢 **Company Overview**
- Name: {company_name}
- Ticker: {ticker}
- Industry: {industry}
- Revenue: {financials['revenue']}B
- Net Income: {financials['net_income']}B
- EPS: {financials['eps']}
- Free Cash Flow: {financials['free_cash_flow']}B
- Region: {}

📰 **Recent Company News**
{company_news}

🏭 **Sector News**
{sector_news}

📊 **Macroeconomic Trends**
{macro_news}

🏛️ **Political Landscape**
{political_news}

🧠 **Analytical Angle**
- {angle}

🎯 **Task:**
Write an insightful article about the current state and sentiment of the company, considering its recent financials, industry positioning, and external influences.

📦 **Output Format (JSON only)**:
```json
{{
    "headline": "A short, informative headline",
    "article": "A concise, structured article of at least two paragraphs"
}}
"""

def generate():
    try:
        ticker = random.choice(TICKERS)
        logger.info("Generating company article for ticker: %s", ticker)

        company_profile = financial_times.fetch_company_profile(ticker)
        financials = financial_times.fetch_company_financials(ticker)

        sector_articles = financial_times.fetch_sector_articles(company_profile.industry)
        company_articles = financial_times.fetch_company_articles(ticker)
        macro_articles = financial_times.fetch_macro_articles()
        political_articles = financial_times.fetch_political_articles()

        prompt = __build_prompt(
            company_name=company_profile.name,
            ticker=company_profile.ticker,
            industry=company_profile.industry,
            financials=financials,
            company_articles=company_articles,
            sector_articles=sector_articles,
            macro_trends=macro_articles,
            political_trends=political_articles
        )

        article_data = execute_prompt(prompt)

        article = CompanyArticle(
            ticker=ticker,
            industry=company_profile.industry,
            company_name=company_profile.name,
            type=0,
            headline=article_data.get("headline", ""),
            content=article_data.get("article", ""),
            metadata={"source": "AI", "reasoning": article_data.get("reasoning", "")}
        )

        financial_times.publish_article(article)
        logger.info("✅ Article for %s published successfully.", ticker)

    except Exception as e:
        logger.exception("❌ Error generating company article for ticker: %s", ticker)
        raise
