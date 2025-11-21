import numpy as np
from typing import List, Dict, Tuple
from datetime import datetime, timedelta, UTC
from inference.sentiment_engine import infer
from services import financial_atlas, financial_times
from models.financial_atlas import PublishAnalysisRequest
import random
import logging

logger = logging.getLogger("worker_app")

def analyze_text(text: str) -> Dict[str, float]:
    """Analyze a single text and return sentiment scores."""
    sentiment, result = infer(text)
    return {
        'positive': result['positive'],
        'negative': result['negative'],
        'neutral': result['neutral'],
        'net_sentiment': result['positive'] - result['negative']
    }

def analyze_articles(articles: List) -> List[Tuple[datetime, Dict[str, float]]]:
    """Analyze multiple articles and return time-stamped sentiments."""
    results = []
    for article in articles:
        # Combine headline and content for better context
        text = f"{article.headline}. {article.content[:1000]}"  # Limit content length
        sentiment = analyze_text(text)
        results.append((article.date or datetime.now(), sentiment))
    return results

def calculate_time_weighted_sentiment(
    sentiments: List[Tuple[datetime, Dict[str, float]]], 
    days_back: int,
    current_date: datetime = None
) -> float:
    """
    Calculate weighted sentiment for a time period.
    More recent articles get higher weight using exponential decay.
    """
    if not sentiments:
        return 0.0
    
    if current_date is None:
        current_date = datetime.now(UTC)
    
    cutoff_date = current_date - timedelta(days=days_back)
    
    weighted_scores = []
    weights = []
    
    for date, sentiment in sentiments:
        if date >= cutoff_date:
            # Calculate days ago (more recent = closer to 0)
            days_ago = (current_date - date).days
            # Exponential decay: recent articles weighted more heavily
            weight = np.exp(-days_ago / (days_back / 2))
            
            weighted_scores.append(sentiment['net_sentiment'] * weight)
            weights.append(weight)
    
    if not weights:
        return 0.0
    
    return sum(weighted_scores) / sum(weights)

def normalize_score(value: float, low: float, high: float) -> float:
    """Normalize a value to [-1, 1] range."""
    if value < low:
        return -1.0
    elif value > high:
        return 1.0
    else:
        # Linear interpolation between low and high
        return 2 * (value - low) / (high - low) - 1

def analyze_financial_statement(financial_statement) -> float:
    """
    Analyze financial statement fundamentals.
    Returns a score from -1 to +1 based on financial health.
    """
    income = financial_statement.income_statement
    balance = financial_statement.balance_sheet
    cashflow = financial_statement.cash_flow_statement
    
    scores = []
    
    # Revenue and profit growth indicators
    if income.revenue_b > 0:
        profit_margin = income.net_income_b / income.revenue_b
        scores.append(normalize_score(profit_margin, 0.1, 0.3))
    
    # Cash flow health
    if cashflow.operating_cash_flow_b > 0:
        fcf_margin = cashflow.free_cash_flow_b / cashflow.operating_cash_flow_b
        scores.append(normalize_score(fcf_margin, 0.3, 0.7))
    
    # Balance sheet strength
    debt_to_equity = balance.debt_to_equity_ratio
    # Lower debt-to-equity is better
    scores.append(normalize_score(2.0 - debt_to_equity, 0.5, 1.5))
    
    # EPS as profitability indicator
    if income.eps_usd > 0:
        scores.append(min(income.eps_usd / 10, 1.0))  # Normalize
    
    return np.mean(scores) if scores else 0.0
    
def generate_analysis(
    ticker: str,
    company_profile,
    financial_statement,
    sector_articles: List,
    company_articles: List,
    macro_articles: List,
    political_articles: List,
    market_data
) -> Dict:
    """
    Generate comprehensive analysis with time-based sentiment scores.
    """
    current_date = datetime.now(UTC)
    
    company_sentiments = analyze_articles(company_articles)
    sector_sentiments = analyze_articles(sector_articles)
    macro_sentiments = analyze_articles(macro_articles)
    political_sentiments = analyze_articles(political_articles)
    
    # Combine all sentiments for overall analysis
    all_sentiments = (
        company_sentiments + 
        sector_sentiments + 
        macro_sentiments + 
        political_sentiments
    )
    
    # Calculate time-based sentiment scores
    current_sentiment = calculate_time_weighted_sentiment(all_sentiments, 3, current_date)
    one_week_sentiment = calculate_time_weighted_sentiment(all_sentiments, 7, current_date)
    one_month_sentiment = calculate_time_weighted_sentiment(all_sentiments, 30, current_date)
    three_month_sentiment = calculate_time_weighted_sentiment(all_sentiments, 90, current_date)
    
    # Weight different article types
    company_weight = 0.4
    sector_weight = 0.25
    macro_weight = 0.2
    political_weight = 0.15
    
    # Calculate weighted current sentiment with different article types
    weighted_current = (
        company_weight * calculate_time_weighted_sentiment(company_sentiments, 3, current_date) +
        sector_weight * calculate_time_weighted_sentiment(sector_sentiments, 7, current_date) +
        macro_weight * calculate_time_weighted_sentiment(macro_sentiments, 14, current_date) +
        political_weight * calculate_time_weighted_sentiment(political_sentiments, 14, current_date)
    )
    
    fundamental_score = analyze_financial_statement(financial_statement)
    
    # Combine sentiment and fundamentals (60% sentiment, 40% fundamentals)
    combined_scores = {
        "current": weighted_current * 0.6 + fundamental_score * 0.4,
        "1w": one_week_sentiment * 0.7 + fundamental_score * 0.3,
        "1m": one_month_sentiment * 0.8 + fundamental_score * 0.2,
        "3m": three_month_sentiment * 0.9 + fundamental_score * 0.1
    }
    
    # Calculate target price based on current price and sentiment
    current_price = market_data.current.stock_price_usd
    
    # Estimate price movement: -1 sentiment = -30% price, +1 sentiment = +30% price
    # Using 3-month outlook as primary driver
    expected_return = combined_scores["3m"] * 0.30
    target_price = current_price * (1 + expected_return)
    
    return {
        "ticker": ticker,
        "current": combined_scores["current"],
        "1w": combined_scores["1w"],
        "1m": combined_scores["1m"],
        "3m": combined_scores["3m"],
        "target_price": round(target_price, 2),
        "metadata": {
            "fundamental_score": fundamental_score,
            "current_price": current_price,
            "article_counts": {
                "company": len(company_articles),
                "sector": len(sector_articles),
                "macro": len(macro_articles),
                "political": len(political_articles)
            }
        }
    }

# Usage in your generate function
def generate():
    tickers = financial_atlas.fetch_tickers()
    if len(tickers) == 0:
        return
    
    ticker = random.choice(tickers)
    logger.info(f"Analyzing {ticker}...")
    
    company_profile = financial_atlas.fetch_company(ticker)
    financial_statement = financial_atlas.fetch_company_financials(ticker)
    market_data = financial_atlas.fetch_market_data(ticker)
    
    sector_articles = financial_times.fetch_sector_articles(company_profile.industry)
    company_articles = financial_times.fetch_company_articles(ticker)
    macro_articles = financial_times.fetch_macro_articles_by_region(company_profile.region)
    political_articles = financial_times.fetch_political_articles_by_region(company_profile.region)
    
    # Generate analysis
    analysis = generate_analysis(
        ticker=ticker,
        company_profile=company_profile,
        financial_statement=financial_statement,
        sector_articles=sector_articles,
        company_articles=company_articles,
        macro_articles=macro_articles,
        political_articles=political_articles,
        market_data=market_data
    )
    
    # Convert to PublishAnalysisRequest format
    request = {
        "ticker": analysis["ticker"],
        "current": float(analysis["current"]),
        "1w": float(analysis["1w"]),
        "1m": float(analysis["1m"]),
        "3m": float(analysis["3m"]),
        "target_price": float(analysis["target_price"])
    }
    
    # Post to external service
    financial_atlas.publish_analysis(PublishAnalysisRequest(**request))
