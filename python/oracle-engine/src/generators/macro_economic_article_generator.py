import random
import helpers.article_formatter as article_formatter
from models.financial_times_models import PoliticalArticle, MacroArticle
from constants import REGIONS
from ai.llm_client import execute_prompt

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

def generate(
    macro_economic_trends: list[MacroArticle], 
    political_landscape: list[PoliticalArticle]
) -> MacroArticle:
    """
    Generates a macroeconomic article considering economic indicators and political factors.
    """
    angle = random.choice(MACROECONOMIC_TOPICS)
    macro_summary = article_formatter.format(macro_economic_trends)
    political_summary = article_formatter.format(political_landscape)
    region = random.choice(REGIONS)

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

    article_data = execute_prompt(prompt)

    return MacroArticle(
        region=region,
        type=1,
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        # metadata={"reasoning": article_data["reasoning"]}
    )
