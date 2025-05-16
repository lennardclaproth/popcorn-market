import logging
import random
from models.generator import Generator
from core import article_formatter
from models.financial_times import PoliticalArticle, MacroArticle
from services import financial_times
from constants.prompt import REGIONS
from services.chat import execute_prompt

generator = Generator(
    active=True,
    description="Generates a macro economic article",
    representation="macro_economic_article_generator",
    name="Macro economic article generator",
    probability=1.0
)

REPRESENTATION = generator.representation

logger = logging.getLogger("worker_app")

MACROECONOMIC_TOPICS = [
    # Neutral/critical angles
    "Focus on inflation trends and central bank policy",
    "Emphasize the effect of political instability on investor confidence",
    "Analyze the role of supply chain disruptions in global trade",
    "Explore how geopolitical tensions affect commodity prices",
    "Discuss the impact of fiscal policy on economic recovery",
    "Examine labor market trends and employment statistics",
    "Analyze capital flows, foreign investment, and currency volatility",
    "Evaluate economic resilience in the face of climate-related policies",

    # ✅ Positive / Opportunity-driven angles
    "Highlight signs of economic recovery and renewed consumer confidence",
    "Explore the role of innovation and technology in boosting productivity",
    "Analyze investment opportunities in emerging markets",
    "Emphasize strong GDP growth and fiscal stability in key regions",
    "Examine positive job market momentum and wage growth trends",
    "Discuss how green energy initiatives are creating new economic pathways",
    "Focus on global cooperation efforts to stabilize financial systems",
    "Explore how digital transformation is driving sustainable economic growth",
]

def __build_prompt(
    macro_economic_trends: list[MacroArticle], 
    political_landscape: list[PoliticalArticle],
    region: str
) -> MacroArticle:
    """
    Generates a macroeconomic article considering economic indicators and political factors.
    """
    angle = random.choice(MACROECONOMIC_TOPICS)
    macro_summary = article_formatter.format(macro_economic_trends)
    political_summary = article_formatter.format(political_landscape)

    prompt = f"""
🔹 **Generate a Unique Macroeconomic Article** 🔹

🌍 **Regional Focus**: {region}

📊 **Macroeconomic Trends:**
{macro_summary}

🏛️ **Relevant Political Developments:**
{political_summary}

🧠 **Analytical Angle:**
- {angle}

🎯 **Task:**
Write a concise, insightful article that reflects recent macroeconomic conditions in {region}, while factoring in the political landscape.

📦 **Output Format (JSON only):**
Return only the following JSON **without commentary, reasoning, or markdown formatting**:
```json
{{
    "headline": "A short, attention-grabbing headline",
    "article": "A well-structured, concise article with at least 2-3 paragraphs."
}}
"""

    return prompt

def generate():
    """
    Generates a new macro economic article
    """
    try:
        logger.info("Started generating a new macro economic article")

        region = random.choice(REGIONS)

        macro_economic_articles = financial_times.fetch_macro_articles_by_region(region)
        political_articles = financial_times.fetch_political_articles_by_region(region)

        prompt = __build_prompt(
            macro_economic_articles,
            political_articles,
            region
        )
        article_data = execute_prompt(prompt)
        article = MacroArticle(
            region=region,
            type=1,
            headline=article_data.get("headline", ""),
            content=article_data.get("article", ""),
            # metadata={"reasoning": article_data["reasoning"]}
        )

        financial_times.publish_article(article)
        logger.info("Successfully published a new macro economic article.")
    except Exception as e:
        raise Exception("An exception occurred while trying to generate a macro economic article.") from e
