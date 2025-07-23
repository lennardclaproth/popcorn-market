# tests/app/generators/test_impact_scorer.py

from datetime import datetime, timedelta
from unittest.mock import patch
from app.generators.financial_report_generator import analyze_sentiment, estimate_impact, generate
from app.models.financial_times import ArticleBase, CompanyArticle, PoliticalArticle, MacroArticle, SectorArticle
from app.models.financial_atlas import Company, MarketSnapshot
from app.constants.financial_times import COMPANY_ARTICLE_TYPE, POLITICAL_ARTICLE_TYPE, MACRO_ARTICLE_TYPE, SECTOR_ARTICLE_TYPE

def test_estimate_impact_high_impact_article():
    article = ArticleBase(
        type=COMPANY_ARTICLE_TYPE,
        headline="Company grows profits in tough economy",
        content="Revenue increased by 8%. Macroeconomic slowdown noted, but profits improved.",
        date=datetime.now()
    )

    score = estimate_impact(article)
    assert 3.0 <= score <= 5.0

def test_estimate_impact_low_impact_article():
    article = ArticleBase(
        type=POLITICAL_ARTICLE_TYPE,
        headline="Minister attends health conference",
        content="No major decisions made. The event was mostly ceremonial.",
        date=datetime.now()
    )

    score = estimate_impact(article)
    assert 1.0 <= score <= 2.5

def test_analyze_sentiment_neutral_article():
    article = ArticleBase(
        type=COMPANY_ARTICLE_TYPE,
        headline="Central Logistics Updates Internal Operations Framework",
        content="""Central Logistics Inc., a regional transportation and warehousing firm based in the Midwest, recently announced updates to its internal operations framework following a scheduled quarterly review. The company confirmed the changes are procedural in nature and part of an ongoing process improvement initiative aimed at aligning operations across its distribution hubs.

The updated framework includes revisions to staff reporting structures, implementation of a standardized digital inventory management interface, and the consolidation of internal auditing procedures across state lines. According to internal communications, no material changes to staffing, strategic priorities, or client-facing processes are expected as a result of these adjustments.

Company leadership noted that the revisions reflect broader administrative harmonization, following a review of operational redundancies identified during a recent efficiency assessment conducted by an external consulting firm. The firm emphasized that these changes are part of routine internal optimization and are not related to any external market forces or financial restructuring.

Central Logistics plans to continue its regular cadence of operational evaluations every quarter, with the next cycle scheduled for late Q3 2025. No additional announcements or press briefings are expected at this time.""",
        date=datetime.now()
    )
    result = analyze_sentiment(article)
    assert result == 0.0


def test_analyze_sentiment_positive_article():
    article = ArticleBase(
        type=COMPANY_ARTICLE_TYPE,
        headline="Siam Wellness Collective Navigates Asia’s Shifting Sands: Steady Growth Amidst Macroeconomic Headwinds",
        content="""Siam Wellness Collective (SIAM), a leading healthcare provider rooted in Chiang Mai, is demonstrating resilience despite a challenging macroeconomic environment across Asia. Trading at 350.44 with a market cap of 428.03, the company’s recent performance reflects a proactive response to the broader slowdown. While Asia’s recovery is stalling due to shifting investment flows and uneven growth rates – largely influenced by geopolitical factors and the UAE’s economic surge – Siam Wellness is maintaining a steady trajectory, driven primarily by its unique blend of traditional wellness practices and modern healthcare solutions.

The company's focus on leveraging local, vibrant traditions provides a competitive advantage, allowing them to cater to a growing market segment seeking authentic wellbeing experiences. Recent financial reports indicate a 7% increase in revenue year-over-year, primarily fueled by expansion into digital wellness offerings and partnerships with local communities.

However, the company isn’t immune to the broader headwinds. The ‘Asia’s Recovery Stalls’ trend, coupled with the increased scrutiny on Oceania’s tech boom due to political uncertainties, presents a risk. Furthermore, while the UAE's economic diversification and investment are bolstering regional growth, they are also intensifying competition for investment capital. Siam Wellness Collective’s strategy of focusing on niche markets within Asia, particularly those aligned with its core values of traditional wellness, offers a buffer against this turbulence. The company’s future success hinges on its continued ability to adapt to evolving geopolitical dynamics and maintain its competitive edge through innovation and community engagement.""",
        date=datetime.now()
    )

    result = analyze_sentiment(article)
    assert result > 0.0 and result <= 1.0

def test_analyze_sentiment_negative_article():
    article = ArticleBase(
        type=COMPANY_ARTICLE_TYPE,
        headline="North America Navigates Persistent Inflation and Uneven Recovery Amidst Fragmented Global Cooperation",
        content="""North America – encompassing the United States and Canada – continues to grapple with elevated inflation, though the pace of increases has demonstrably slowed compared to 2022. The Federal Reserve's aggressive interest rate hikes have begun to exert downward pressure on demand, but core inflation remains stubbornly high, largely driven by persistent labor shortages and supply chain bottlenecks, albeit diminishing. While GDP growth has shown signs of stabilization, its trajectory remains fragile, vulnerable to potential recessionary shocks. The differing approaches between the U.S. and Canadian central banks – with the Bank of Canada maintaining a slightly more hawkish stance – highlight the challenges of coordinated monetary policy in a world of diverging economic priorities.

Politically, the current administration in the United States is prioritizing fiscal discipline while simultaneously pushing for infrastructure investments and clean energy initiatives. This delicate balancing act is further complicated by upcoming midterm elections, creating an environment of policy uncertainty. Canada, facing similar electoral pressures, is navigating a debate surrounding carbon taxes and resource development, with significant implications for its economy. The Biden administration in the US has actively sought to revitalize international cooperation, particularly through the G7 and IMF, advocating for coordinated action on debt restructuring for vulnerable emerging economies.

Despite recent improvements, the overall picture is one of uneven recovery. Global cooperation efforts, while present, have largely been reactive rather than proactive. The IMF's recent focus on debt sustainability and the ongoing discussions regarding the बेलarus debt crisis underscore the limitations of current mechanisms. The fragmented nature of international collaboration – driven by geopolitical tensions and nationalistic economic policies – continues to present a significant headwind for North American economic stability, necessitating a renewed push for robust, multilateral solutions to systemic risks.""",
        date=datetime.now()
    )
    result = analyze_sentiment(article)
    assert result < 0.0 and result >= -1.0

@patch("services.financial_atlas.fetch_tickers")
@patch("services.financial_atlas.fetch_company")
@patch("services.financial_atlas.fetch_current_market_snapshot")
@patch("services.financial_times.fetch_sector_articles")
@patch("services.financial_times.fetch_company_articles")
@patch("services.financial_times.fetch_macro_articles_by_region")
@patch("services.financial_times.fetch_political_articles_by_region")
def test_generate(
    mock_political_articles_by_region,
    mock_macro_articles_by_region,
    mock_company_articles,
    mock_sector_articles,
    mock_snapshot,
    mock_company,
    mock_tickers
):
    # ARRANGE

    mock_tickers.return_value = ['ACME']

    # Mock Company
    mock_company.return_value = Company(
        ticker="ACME",
        name="Acme Corp",
        industry="Technology",
        description="Makes gadgets.",
        headquarters="Silicon Valley",
        ceo="Jane Doe",
        founded_year=2000,
        employees=5000,
        region="North America"
    )

    # Mock Market Snapshot
    mock_snapshot.return_value = MarketSnapshot(
        stock_price_USD=150.5,
        volume=1000000,
        market_cap_B=75.2,
        dividend_per_share_usd=1.2,
        dividend_yield_percent=2.1,
        date=datetime.now()
    )

    # Mock sector article
    mock_sector_articles.return_value = [
        SectorArticle(
            id="1",
            type=SECTOR_ARTICLE_TYPE,
            headline="Technology Sector Maintains Cautious Optimism Amid Global Supply Constraints",
            content="Despite ongoing supply chain disruptions and regulatory scrutiny in key markets, the technology sector continues to post moderate growth. Analysts predict stable output through Q4, driven by cloud services and AI investments.",
            sector="Technology",
            region="North America",
            publish_date=datetime.now() - timedelta(days=12)
        )
    ]

    # Mock company article
    mock_company_articles.return_value = [
        CompanyArticle(
            id="2",
            type=COMPANY_ARTICLE_TYPE,
            headline="Acme Corp Beats Expectations with Strong Q2 Earnings",
            content="Acme Corp reported a 10% increase in net profit, citing strong demand for its cybersecurity products and expanding cloud offerings. The CEO noted that the company is investing heavily in AI infrastructure to maintain momentum.",
            ticker="ACME",
            sector="Technology",
            company_name="Acme Corp",
            publish_date=datetime.now() - timedelta(days=65)
        )
    ]

    # Mock macroeconomic article
    mock_macro_articles_by_region.return_value = [
        MacroArticle(
            id="3",
            type=MACRO_ARTICLE_TYPE,
            headline="North American Economy Holds Steady Despite Interest Rate Hikes",
            content="Recent data shows GDP growth stabilizing as central banks ease off aggressive monetary tightening. Inflation remains within target range, though labor market imbalances persist.",
            region="North America",
            publish_date=datetime.now() - timedelta(days=5)
        )
    ]

    # Mock political article
    mock_political_articles_by_region.return_value = [
        PoliticalArticle(
            id="4",
            type=POLITICAL_ARTICLE_TYPE,
            headline="Uncertainty Ahead as U.S. Midterm Elections Approach",
            content="With elections looming, potential shifts in fiscal and trade policies are raising concerns among investors. Analysts warn of possible delays in tech regulation and stimulus measures depending on the outcome.",
            region="North America",
            publish_date=datetime.now()
        )
    ]
            
    generate()