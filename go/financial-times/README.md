# Popcorn financial times

Popcorn financial times is a WebApi that serves to deliver all the news of the popcorn market. It should be able to receive new news articles to publish and then also send out messages that there are new articles available.

Within the popcorn financial times there are different types of articles, these articles are:
- Company specific news articles
- Sector specific news articles
- Articles about macro economic trends
- Articles about global political trends

## Company specific news articles
Company specific news articles are articles that report on news of companies. It is important that we should be able to get a list of companies from the financial times application, their financials but also the news articles of companies.

**The model of a compane news article looks as follows:**
```json
{
    "ticker": "ticker",
    "industry": "Industry",
    "company_name": "Company name",
    "date": "publishing date",
    "article": "article content",
    "headline": "article headline",
    "metadata": {
        "reasoning": "The reasoning the model has done to com to its conclusion"
    },
}
```