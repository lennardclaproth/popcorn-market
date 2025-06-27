from itertools import chain
import logging
import random
from constants.financial_times import SERVICE_DESC
from core import article_formatter
from models.financial_times import CompanyArticle, SectorArticle, PoliticalArticle, MacroArticle, ArticleBase
from models.financial_atlas import Company, MarketSnapshot
from models.generator import Generator
from models.graph import NodeMetadata
from services import financial_times, financial_atlas, graph, chat
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
    company: Company,
    snapshot: MarketSnapshot,
    company_articles: list[CompanyArticle],
    sector_articles: list[SectorArticle],
    macro_trends: list[MacroArticle],
    macro_trends_by_region: list[MacroArticle],
    political_trends: list[PoliticalArticle],
    political_trends_by_region: list[PoliticalArticle],
    region: str
) -> str:
    angle = random.choice(COMPANY_ANGLES)

    company_news = article_formatter.format(company_articles)
    sector_news = article_formatter.format(sector_articles)
    macro_news = article_formatter.format(macro_trends)
    macro_news_by_region = article_formatter.format(macro_trends_by_region)
    political_news = article_formatter.format(political_trends)
    political_news_by_region = article_formatter.format(political_trends_by_region)
    macro_news_by_region = article_formatter.format(macro_trends_by_region)

    return f"""
🔹 **Generate a Company Article** 🔹

🌍 **Regional Focus**: {region}

🏢 **Company Overview**
- Name: {company.name}
- Ticker: {company.ticker}
- Industry: {company.industry}
- Description: {company.description}
- Region: {company.region}
- Stock price: {snapshot.stock_price_usd}
- Market cap B: {snapshot.market_cap_b}

📰 **Recent Company News**
{company_news}

🏭 **Sector News**
{sector_news}

📊 **Macroeconomic Trends**
{macro_news}

📊 **Macroeconomic Trends by region**
{macro_news_by_region}

🏛️ **Political Landscape**
{political_news}

🏛️ **Political Landscape by region**
{political_news_by_region}

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
        tickers = financial_times.fetch_tickers()

        if len(tickers) == 0:
            return
        
        ticker = random.choice(tickers)
        logger.info("Generating company article for ticker: %s", ticker)

        company_profile = financial_atlas.fetch_company(ticker)
        snapshot = financial_atlas.fetch_current_market_snapshot(ticker)

        sector_articles = financial_times.fetch_sector_articles(company_profile.industry)
        company_articles = financial_times.fetch_company_articles(ticker)

        macro_articles = financial_times.fetch_macro_articles()
        macro_articles_by_region = financial_times.fetch_macro_articles_by_region(company_profile.region)
        regional_ids = {article.id for article in macro_articles_by_region}
        macro_articles = [article for article in macro_articles if article.id not in regional_ids]

        political_articles = financial_times.fetch_political_articles()
        political_articles_by_region = financial_times.fetch_political_articles_by_region(company_profile.region)
        regional_ids = {article.id for article in political_articles_by_region}
        political_articles = [article for article in macro_articles if article.id not in regional_ids]

        prompt = __build_prompt(
            company=company_profile,
            snapshot=snapshot,
            company_articles=company_articles,
            sector_articles=sector_articles,
            macro_trends=macro_articles,
            macro_articles_by_region=macro_articles_by_region,
            political_trends=political_articles,
            political_articles_by_region=political_articles_by_region,
            macro_trends_by_region=macro_articles_by_region,
            region=company_profile.region
        )

        article_data = chat.execute_prompt(prompt)

        article = CompanyArticle(
            ticker=ticker,
            company_name=company_profile.name,
            sector=company_profile.industry,
            type=0,
            headline=article_data.get("headline", ""),
            content=article_data.get("article", ""),
            metadata={"source": "AI", "reasoning": article_data.get("reasoning", "")}
        )

        entity_id = financial_times.publish_article(article)
        logger.info("✅ Article for %s published successfully.", ticker)

        # Builds a node based on the information gathered
        logger.info("Generating Node for entity with Id: %s.", entity_id)
        children = [article.id for article in chain(political_articles, sector_articles, macro_articles, company_articles, political_articles)]
        graph.create_node(entity_id, NodeMetadata(service=SERVICE_DESC), children)
        logger.info("✅ Successfully inserted Node with entity_id %s.", entity_id)

    except Exception as e:
        logger.exception("❌ Error generating company article for ticker: %s", ticker)
        raise
