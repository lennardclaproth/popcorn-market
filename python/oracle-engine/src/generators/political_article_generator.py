import datetime
from models.financial_times_models import PoliticalArticle
from ai.llm_client import execute_prompt

def generate(political_landscape: list[PoliticalArticle]) -> PoliticalArticle:
    """
    Generates a political article based on recent political developments.
    """

    prompt = f"""
    🔹 **Generate a Political Article** 🔹

    **Political Developments**
    {political_landscape}

    ### **Task:**
    Write a **concise, informative political article**.

    🔹 **Output Format:** Return a JSON object:
    ```json
    {{
        "headline": "A short, engaging headline",
        "article": "A concise article"
    }}
    ```
    """

    article_data = execute_prompt(prompt)

    return PoliticalArticle(
        region="Global",
        date=datetime.datetime.now(),
        type="Political Analysis",
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        metadata={"reasoning": article_data["reasoning"]}
    )
