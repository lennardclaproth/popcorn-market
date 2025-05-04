from constants import REGIONS
import helpers.article_formatter as article_formatter
from models.financial_times_models import PoliticalArticle
from ai.llm_client import execute_prompt
import random

POLITICAL_TOPICS = [
    # 🔍 Critical / investigative / tension-oriented topics
    "Focus on diplomatic tensions and international relations",
    "Emphasize the economic impact of political decisions",
    "Analyze public response and sentiment",
    "Explore party politics and internal government conflict",
    "Discuss leadership changes and power dynamics",
    "Examine civil rights and legal reforms",
    "Cover protests, movements, and activism",
    "Report on foreign policy decisions and global strategy",
    "Highlight corruption, investigations, or political scandals",
    "Evaluate election campaigns and voter behavior",
    "Discuss security policy, defense spending, or cyber policy",
    "Explore environmental policies and political accountability",
    "Discuss influence of political lobbying or think tanks",
    "Analyze media influence and propaganda in politics",
    "Investigate judicial involvement or Supreme Court rulings",

    # ✅ Positive / constructive / progress-driven topics
    "Highlight successful bipartisan collaboration or policy compromise",
    "Explore the rise of civic engagement and democratic participation",
    "Report on landmark legal reforms that expanded rights or protections",
    "Discuss the role of young leaders and political innovation",
    "Celebrate peaceful transitions of power or democratic stability",
    "Examine international cooperation on climate, trade, or peacekeeping",
    "Highlight political efforts advancing education and public health",
    "Showcase grassroots movements driving positive systemic change",
    "Analyze how data-driven governance improves transparency and trust",
    "Explore inclusive policymaking that bridges social and economic divides",
]


def generate(political_landscape: list[PoliticalArticle]) -> PoliticalArticle:
    """
    Generates a political article based on recent political developments.
    """

    # Get topic hint
    topic_hint = random.choice(POLITICAL_TOPICS)

    region = random.choice(REGIONS)

    # Generate summaries
    summary_string = article_formatter.format(political_landscape)

    prompt = f"""
🔹 **Generate a Unique Political News Article** 🔹

🌍 **Regional Focus**: {region}

🗺️ **Political Context (Recent Articles):**
{summary_string}

🎯 **Your Task:**
Write a **concise, engaging political article** that reflects recent developments in {region} from the context above.

📌 **Angle to Take**:
- {topic_hint}

📦 **Output Format (JSON only):**
Return only the following JSON **without commentary, reasoning, or markdown formatting**:
```json
{{
    "headline": "A short, attention-grabbing headline",
    "article": "A well-structured, concise article with at least 2-3 paragraphs."
}}
"""

    article_data = execute_prompt(prompt)

    return PoliticalArticle(
        region="Global",
        type=2,
        headline=article_data.get("headline", ""),
        content=article_data.get("article", ""),
        # metadata={"reasoning": article_data["reasoning"]}
    )
