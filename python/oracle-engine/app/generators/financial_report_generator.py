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
from sqlite3 import Date
from models.financial_atlas import BalanceSheet, CashFlowStatement, FinancialStatement, IncomeStatement, MarketData, PeriodType, PublishFinancialStatementRequest, ReportingPeriod
from services import financial_atlas, financial_times, graph
from models.financial_times import ArticleBase
from models.generator import Generator
from constants.financial_times import COMPANY_ARTICLE_TYPE, MACRO_ARTICLE_TYPE, POLITICAL_ARTICLE_TYPE, SECTOR_ARTICLE_TYPE
from constants.financial_atlas import SERVICE_DESC
from models.graph import NodeMetadata, EventType
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


def generate():
    """
    
    """
    tickers = financial_atlas.fetch_tickers()
    if len(tickers) == 0:
        return

    ticker = random.choice(tickers)
    company_profile = financial_atlas.fetch_company(ticker)
    financial_statement = financial_atlas.fetch_company_financials(ticker)
    
    # I don't know if this is nice...
    if financial_statement is None:
        market_data = financial_atlas.fetch_market_data(ticker)
        initial_financial_statement = generate_initial_financial_statement(market_data)
        entity_id = financial_atlas.publish_financial_statement(
            financial_statement=initial_financial_statement
        )
        graph.create_node(entity_id, NodeMetadata(service=SERVICE_DESC, event_type=EventType.FINANCIAL_STATEMENT_PUBLISHED), [])
        return

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
    financial_statement = apply_growth_factor(financial_statement, growth_factor)
    entity_id = financial_atlas.publish_financial_statement(financial_statement)

    children = [article.id for article in all_articles]
    graph.create_node(entity_id, NodeMetadata(service=SERVICE_DESC, event_type=EventType.FINANCIAL_STATEMENT_PUBLISHED, kvp={"growth_factor": growth_factor + 1}), children)


def generate_initial_financial_statement(market_data: MarketData) -> PublishFinancialStatementRequest:
    snapshot = market_data.current
    shares = market_data.shares_outstanding
    market_cap = snapshot.market_cap_b

    # Derived values
    revenue = market_cap * 1.5
    cogs = revenue * 0.5
    gross_profit = revenue - cogs
    operating_expenses = revenue * 0.2
    ebitda = gross_profit - operating_expenses
    depreciation_amortization = revenue * 0.05
    ebit = ebitda - depreciation_amortization
    interest_expense = revenue * 0.02
    tax_rate = 0.2
    net_income = (ebit - interest_expense) * (1 - tax_rate)
    eps = net_income * 1e9 / shares  # convert billions to dollars

    income_statement = IncomeStatement(
        revenue_b=revenue,
        cogs_b=cogs,
        gross_profit_b=gross_profit,
        operating_expenses_b=operating_expenses,
        ebitda_b=ebitda,
        depreciation_amortization_b=depreciation_amortization,
        ebit_b=ebit,
        interest_expense_b=interest_expense,
        tax_rate_percent=tax_rate * 100,
        net_income_b=net_income,
        eps_usd=round(eps, 2),
    )

    total_assets = market_cap * 1.5
    total_liabilities = total_assets * 0.4
    total_equity = total_assets - total_liabilities

    balance_sheet = BalanceSheet(
        total_assets_b=total_assets,
        total_liabilities_b=total_liabilities,
        total_equity_b=total_equity,
        debt_to_equity_ratio=(total_liabilities / total_equity),
    )

    operating_cash_flow = net_income * 0.8
    capex = revenue * 0.05
    free_cash_flow = operating_cash_flow - capex
    financing_cash_flow = market_cap * 0.01
    investing_cash_flow = -capex
    net_cash_flow = operating_cash_flow + financing_cash_flow + investing_cash_flow

    cash_flow_statement = CashFlowStatement(
        operating_cash_flow_b=operating_cash_flow,
        capital_expenditures_b=capex,
        free_cash_flow_b=free_cash_flow,
        financing_cash_flow_b=financing_cash_flow,
        investing_cash_flow_b=investing_cash_flow,
        net_cash_flow_b=net_cash_flow,
    )

    return PublishFinancialStatementRequest(
        ticker=market_data.ticker,
        year=datetime.now(timezone.utc).year,
        interval=PeriodType.Yearly,
        period_number=1,
        income_statement=income_statement,
        balance_sheet=balance_sheet,
        cash_flow_statement=cash_flow_statement,
    )
    
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

def apply_growth_factor(
    previous: PublishFinancialStatementRequest,
    growth_factor: float
) -> PublishFinancialStatementRequest:
    growth_factor = 1 + growth_factor

    if growth_factor <= 0:
        raise ValueError("Growth factor must result in a positive multiplier.")
    
    prev_income = previous.income_statement
    prev_balance = previous.balance_sheet
    prev_cash = previous.cash_flow_statement

    # --- Income Statement ---
    revenue = prev_income.revenue_b * growth_factor
    cogs = revenue * 0.5
    gross_profit = revenue - cogs
    operating_expenses = prev_income.operating_expenses_b * growth_factor
    ebitda = gross_profit - operating_expenses
    depreciation_amortization = prev_income.depreciation_amortization_b * growth_factor
    ebit = ebitda - depreciation_amortization
    interest_expense = prev_income.interest_expense_b * growth_factor
    tax_rate = prev_income.tax_rate_percent / 100
    net_income = (ebit - interest_expense) * (1 - tax_rate)
    eps = net_income * 1e9 / (net_income * 1e9 / prev_income.eps_usd)  # Keep same share count

    income_statement = IncomeStatement(
        revenue_b=revenue,
        cogs_b=cogs,
        gross_profit_b=gross_profit,
        operating_expenses_b=operating_expenses,
        ebitda_b=ebitda,
        depreciation_amortization_b=depreciation_amortization,
        ebit_b=ebit,
        interest_expense_b=interest_expense,
        tax_rate_percent=tax_rate * 100,
        net_income_b=net_income,
        eps_usd=round(eps, 2),
    )

    # --- Balance Sheet ---
    total_assets = prev_balance.total_assets_b * (1 + (growth_factor - 1) * 0.5)
    total_liabilities = total_assets * 0.4  # keep ratio constant
    total_equity = total_assets - total_liabilities

    balance_sheet = BalanceSheet(
        total_assets_b=total_assets,
        total_liabilities_b=total_liabilities,
        total_equity_b=total_equity,
        debt_to_equity_ratio=(total_liabilities / total_equity),
    )

    # --- Cash Flow Statement ---
    operating_cash_flow = net_income * 0.8
    capex = revenue * 0.05
    free_cash_flow = operating_cash_flow - capex
    financing_cash_flow = prev_cash.financing_cash_flow_b * growth_factor
    investing_cash_flow = -capex
    net_cash_flow = operating_cash_flow + financing_cash_flow + investing_cash_flow

    cash_flow_statement = CashFlowStatement(
        operating_cash_flow_b=operating_cash_flow,
        capital_expenditures_b=capex,
        free_cash_flow_b=free_cash_flow,
        financing_cash_flow_b=financing_cash_flow,
        investing_cash_flow_b=investing_cash_flow,
        net_cash_flow_b=net_cash_flow,
    )

    # --- Reporting Period ---
    prev_period = ReportingPeriod.from_string(previous.period)
    next_period = ReportingPeriod.next_period(prev_period)

    return PublishFinancialStatementRequest(
        ticker=previous.ticker,
        year=next_period.year,
        interval=PeriodType.Quarterly,
        period_number=next_period.period_number,
        income_statement=income_statement,
        balance_sheet=balance_sheet,
        cash_flow_statement=cash_flow_statement,
    )

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