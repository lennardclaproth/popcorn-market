def format(articles, max_articles=3) -> str:
    summaries = []
    for article in articles[:max_articles]:
        date_str = article.date.strftime('%Y-%m-%d') if article.date else "Unknown date"
        summary = article.content.strip().replace("\n", " ")[:300]
        summaries.append(
            f"- **{article.headline or 'Untitled'}** ({date_str}): {summary}..."
        )
    return "\n\n".join(summaries)  # ✅ Add blank lines between summaries
