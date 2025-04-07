import datetime
from models.financial_times_models import PoliticalArticle, MacroArticle
from ai.llm_client import execute_prompt

def generate(
    macro_economic_trends: list[MacroArticle], 
    political_landscape: list[PoliticalArticle]
) -> MacroArticle:
    """
    Generates a macroeconomic article considering economic indicators and political factors.
    """

    prompt = f"""
    🔹 **Generate a Macroeconomic Article** 🔹

    **Macroeconomic Trends**
    {macro_economic_trends}

    **Political Landscape**
    {political_landscape}

    ### **Task:**
    Write a **concise, engaging article** summarizing economic conditions.

    🔹 **Output Format:** Return a JSON object:
    ```json
    {{
        "headline": "A short, engaging headline",
        "article": "A concise article"
    }}
    ```
    """

    article_data = execute_prompt(prompt)

    return MacroArticle(
        region="Global",
        date=datetime.datetime.now(),
        type="Macroeconomic Analysis",
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        metadata={"reasoning": article_data["reasoning"]}
    )
