import logging
import random
from constants import REGIONS
from core import article_formatter
from models.financial_times import PoliticalArticle, MacroArticle, SectorArticle
from models.generator import Generator
from services.chat import execute_prompt
from services import financial_times
from constants import SECTORS

logger = logging.getLogger("worker_app")

generator = Generator(
    active=True,
    description="Generates a sector article",
    representation="sector_article_generator",
    name="Sector article generator",
    probability=1.0
)

REPRESENTATION = generator.representation

SECTOR_ANGLES = [
    # 🔍 Analytical / strategic
    "Focus on investor confidence within the sector",
    "Analyze supply chain stability and innovation",
    "Discuss regulatory or legal challenges affecting the sector",
    "Emphasize international competition and market positioning",
    "Explore consumer behavior and demand fluctuations",
    "Evaluate the impact of interest rates or inflation on the sector",
    "Highlight sector resilience or volatility amidst global crises",
    "Report on M&A activity and consolidation trends in the sector",
    
    # ✅ Positive / opportunity-driven
    "Showcase breakthrough innovations and digital transformation in the sector",
    "Highlight strong quarterly growth and positive earnings outlooks",
    "Explore how sustainability initiatives are reshaping the sector",
    "Discuss sector contributions to job creation and economic development",
    "Analyze competitive advantages that are driving long-term value",
    "Emphasize how emerging technologies (AI, automation, etc.) are being adopted",
    
    # 🌍 Global dynamics & transitions
    "Discuss the sector's adaptation to global economic realignments",
    "Explore how shifting trade agreements or geopolitical shifts impact the sector",
    "Analyze how ESG (Environmental, Social, Governance) standards are influencing strategy",
    
    # 🧠 Forward-looking / investment-oriented
    "Highlight investment trends and where institutional capital is flowing",
    "Evaluate risks and opportunities in international market expansion",
    "Explore strategic pivots companies are making to future-proof their business models",
    "Assess how leadership vision and governance shape sector momentum",
]

def __build_prompt(
    sector: str,
    sector_articles: list[SectorArticle], 
    macro_economic_trends: list[MacroArticle], 
    political_landscape: list[PoliticalArticle]
) -> str:
    """
    Generates a sector analysis article using industry trends, economic influences, and politics.
    """
    angle = random.choice(SECTOR_ANGLES)
    region = random.choice(REGIONS)
    sector_summary = article_formatter.format(sector_articles)
    macro_summary = article_formatter.format(macro_economic_trends)
    political_summary = article_formatter.format(political_landscape)
    
    return f"""
🔹 **Generate a Unique Sector Analysis Article** 🔹

🌍 **Regional Focus**: {region}

📈 **Sector: {sector}**

🧩 **Sector-Specific Articles:**
{sector_summary}

📊 **Macroeconomic Trends:**
{macro_summary}

🏛️ **Political Landscape:**
{political_summary}

🧠 **Analytical Angle:**
- {angle}

🎯 **Task:**
Write a **concise and insightful article** that reflects recent trends in the {sector} sector. You must consider the economic and political factors that affect the sector and frame the article based on the analytical angle provided.

📦 **Output Format (JSON only):**
Return only the following JSON **without commentary, reasoning, or markdown formatting**:
```json
{{
    "headline": "A short, attention-grabbing headline",
    "article": "A well-structured, concise article with at least 2-3 paragraphs."
}}
"""

def generate():
    """
    Gets the correct data from the api and generates a new article
    based on that data.
    """

    # Get articles from articles api
    try:
        sector = random.choice(SECTORS)
        logger.info("Starting article generation for sector: %s sector.", sector)

        sector_articles = financial_times.fetch_sector_articles(sector)
        macro_economic_articles = financial_times.fetch_macro_articles()
        political_articles = financial_times.fetch_political_articles()

        prompt = __build_prompt(sector_articles, 
                                macro_economic_articles, 
                                political_articles)
        article_data = execute_prompt(prompt)
        article = SectorArticle(
            sector=sector,
            type=3,
            headline=article_data.get("headline", ""),
            content=article_data.get("article", ""),
            # metadata={"reasoning": article_data["reasoning"]}
        )
        # Generate article via generator service with the inputs
        financial_times.publish_article(article)
        logger.info("Article successfully published.")

    except Exception as e:
        raise Exception("An exception occurred while trying to generate a sector article.") from e
