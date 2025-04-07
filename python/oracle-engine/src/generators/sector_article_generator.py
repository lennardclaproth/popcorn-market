import datetime
from models.financial_times_models import PoliticalArticle, MacroArticle, SectorArticle
from ai.llm_client import execute_prompt

SECTOR_CONSTANT = "Financials"

def generate(
    sector_articles: list[SectorArticle], 
    macro_economic_trends: list[MacroArticle], 
    political_landscape: list[PoliticalArticle]
) -> SectorArticle:
    """
    Generates a sector analysis article using industry trends, economic influences, and politics.
    """

    prompt = f"""
    🔹 **Generate a Sector Analysis Article** 🔹

    **Sector Insights**
    - Recent Articles: {sector_articles}
    - Macroeconomic Trends: {macro_economic_trends}
    - Political Landscape: {political_landscape}

    ### **Task:**
    Write a **concise and engaging article** about the {SECTOR_CONSTANT} sector.

    🔹 **Output Format:** Return a JSON object:
    ```json
    {{
        "headline": "A short, engaging headline",
        "article": "A concise article"
    }}
    ```
    """

    article_data = execute_prompt(prompt)

    return SectorArticle(
        sector=SECTOR_CONSTANT,
        date=datetime.datetime.now(),
        type=3,
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        # metadata={"reasoning": article_data["reasoning"]}
    )
