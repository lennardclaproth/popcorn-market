"""
    File:   financial_report_generator.py
    Author: Lennard Claproth
    Date:   27/06/2025
    
    The financial report generator generates a new financial report.
    It does this by gathering all the relevant articles:
        - CompanyArticles
        - SectorArticles
        - PoliticalArticles (by region)
        - MacroEconomicArticles (by region)

    Doing a sentiment analysis over those articles and giving a weight
    to each article.

    The weight is given as follows:
        The article is given a relevance score based on the following criteria:
            - When was the article published
            - What is the potential impact

    This relevance score together with the sentiment score is used to 
    calculate the overall growth (or shrink) percentage this is then used 
    to calculate the new financials. 

    This will look as follows:
        impactScore = f(article)
            A score between 1 and 5 determined by the type of article and some
            common keywords that might signify a high impact for the article

        sentimentScore = f(article)
            A score between 0 and 1 where < 0.5 is deemed to be negative sentiment
            and > 0.5 positive sentiment. This uses finBERT for the sentiment analysis

        RelevanceScore = f(publishedAt, impactScore)
            Uses time decay based on the days that have passed since the publishing
            of the article which gets multiplied with the impactScore
        
        TotalSentiment = {
            "company": avg(FinalScore for companyArticles),
            "sector": avg(FinalScore for sectorArticles),
            "political": avg(FinalScore for politicalArticles),
            "macro": avg(FinalScore for macroArticles),
        }

        AbsImpact = RelevanceScore * abs(SentimentScore)
        NetImpact = RelevanceScore * SentimentScore
        weight = sum(articleScore in Category) / sum(articleScore across all categories)

        GrowthFactor = (
            Weight * TotalSentiment["company"] +
            Weight * TotalSentiment["sector"] +
            Weight * TotalSentiment["macro"] +
            Weight * TotalSentiment["political"]
        )

    The growth factor is then used as input for the calculation of the financials.
"""

from datetime import datetime, timezone
from math import exp
import random
from models.financial_atlas import FinancialStatement
from services import financial_atlas, financial_times
from models.financial_times import ArticleBase
from models.generator import Generator
from constants.financial_times import COMPANY_ARTICLE_TYPE, MACRO_ARTICLE_TYPE, POLITICAL_ARTICLE_TYPE, SECTOR_ARTICLE_TYPE
from transformers import AutoModelForSequenceClassification, AutoTokenizer, BertTokenizer, BertForSequenceClassification, pipeline
from os.path import dirname

generator = Generator(
    active=True,
    description="Generates a financial report",
    representation="financial_report_generator",
    name="Financial report generator",
    probability=1.0
)

REPRESENTATION = generator.representation

HIGH_IMPACT_KEYWORDS = [
    "bankruptcy", "acquisition", "merger", "restructuring", "regulation",
    "sanction", "lawsuit", "strike", "investigation", "boom", "crash", "layoff",
    "headwind", "resilience", "recovery", "risk", "growth", "surge"
]

FINANCIAL_TERMS = [
    "revenue", "earnings", "profit", "loss", "margin", "cash flow", "market cap", "trading at", "valuation"
]

MACRO_TERMS = [
    "geopolitical", "macroeconomic", "uae", "asia", "oceania", "inflation", "interest rates", "policy"
]

model_name = "ProsusAI/finbert"

tokenizer = AutoTokenizer.from_pretrained(f'{dirname(__file__)}/finbert-local/')
model = AutoModelForSequenceClassification.from_pretrained(f'{dirname(__file__)}/finbert-local/')

finbert = pipeline("sentiment-analysis", model=model, tokenizer=tokenizer)


def generate() -> FinancialStatement:
    """
    
    """
    tickers = financial_atlas.fetch_tickers()
    if len(tickers) == 0:
        return
    
    ticker = random.choice(tickers)

    # Should check if the ticker already has a quarterly report of the current quarter
    # Should check if a quarterly report already exists.

    company_profile = financial_atlas.fetch_company(ticker)
    financial_statement = financial_atlas.fetch_company_financials(ticker)

    sector_articles = financial_times.fetch_sector_articles(company_profile.industry)
    company_articles = financial_times.fetch_company_articles(ticker)
    macro_articles_by_region = financial_times.fetch_macro_articles_by_region(company_profile.region)
    political_articles_by_region = financial_times.fetch_political_articles_by_region(company_profile.region)
    
    all_articles = (
        sector_articles +
        company_articles +
        macro_articles_by_region +
        political_articles_by_region
    )

    growth_factor = determine_growth_factor(all_articles)

    

    
def determine_growth_factor(articles: list[ArticleBase]):
    scores = {
        COMPANY_ARTICLE_TYPE : [],
        POLITICAL_ARTICLE_TYPE : [],
        MACRO_ARTICLE_TYPE : [],
        SECTOR_ARTICLE_TYPE : [],
    }

    for article in articles:
        impact = estimate_impact(article)
        relevance = define_relevance(article, impact)
        sentiment = analyze_sentiment(article)
        abs_score = relevance * abs(sentiment)
        net_score = relevance * sentiment

        score_result = {
                "entity_id": article.id,
                "impact": impact,
                "relevance": relevance,
                "sentiment": sentiment,
                "abs_score": abs_score,
                "net_score": net_score
            }

        scores[article.type].append(score_result)  
    
    # Calculate dynamic weights per category
    abs_totals = {
    category: sum(s["abs_score"] for s in score_list)
        for category, score_list in scores.items()
    }

    total_abs = sum(abs_totals.values())

    weights = {
        category: (abs_totals[category] / total_abs) if total_abs > 0 else 0.0
        for category in scores
    }

    # Compute net totals per category
    net_totals = {
        category: (
            sum(s["net_score"] for s in score_list) / len(score_list)
            if score_list else 0.0
        )
        for category, score_list in scores.items()
    }

    # Weighted sum of net scores
    weighted_sum = sum(
        net_totals[category] * weights[category]
        for category in scores
    )

    # Final growth factor (clamped)
    scaling_factor = 10.0
    cap = 0.25
    growth = max(min(weighted_sum / scaling_factor, cap), -cap)

    return growth


def analyze_sentiment(article: ArticleBase) -> float:
    text = f"{article.headline}. {article.content}"
    result = finbert(text)

    label = result[0]['label']
    confidence = result[0]['score']

    if label == "positive":
        return confidence  
    elif label == "negative":
        return -confidence
    else:  
        return 0.0

def define_relevance(article: ArticleBase, impact_score: float) -> float:
    age_days = (datetime.now(timezone.utc) - article.date).days
    time_decay = exp(-0.05 * age_days)
    return time_decay * impact_score

def estimate_impact(article: ArticleBase) -> float:
    """
    estimates the impact of an article based on the category and certain key
    words in the headline.
    """
    score = 1.0

    type_score = {
        COMPANY_ARTICLE_TYPE: 1.5,
        SECTOR_ARTICLE_TYPE: 1.0,
        MACRO_ARTICLE_TYPE: 0.8,
        POLITICAL_ARTICLE_TYPE: 0.6
    }
    score += type_score.get(article.type, 0.5)

    if any(word in article.headline.lower() for word in HIGH_IMPACT_KEYWORDS):
        score += 0.5

    if any(term in article.content.lower() for term in FINANCIAL_TERMS):
        score += 0.7

    if any(term in article.content.lower() for term in MACRO_TERMS):
        score += 0.7

    word_count = len(article.content.split())
    if word_count > 250:
        score += 0.3
    elif word_count > 150:
        score += 0.15

    return min(score, 5.0)