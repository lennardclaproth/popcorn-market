import random
from constants import REGIONS
import helpers.article_formatter as article_formatter
from models.financial_times_models import PoliticalArticle, MacroArticle, SectorArticle
from ai.llm_client import execute_prompt
from constants import SECTORS

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

def generate(
    sector: str,
    sector_articles: list[SectorArticle], 
    macro_economic_trends: list[MacroArticle], 
    political_landscape: list[PoliticalArticle]
) -> SectorArticle:
    """
    Generates a sector analysis article using industry trends, economic influences, and politics.
    """
    angle = random.choice(SECTOR_ANGLES)
    region = random.choice(REGIONS)
    sector_summary = article_formatter.format(sector_articles)
    macro_summary = article_formatter.format(macro_economic_trends)
    political_summary = article_formatter.format(political_landscape)
    
    prompt = f"""
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

    article_data = execute_prompt(prompt)

    return SectorArticle(
        sector=sector,
        type=3,
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        # metadata={"reasoning": article_data["reasoning"]}
    )
