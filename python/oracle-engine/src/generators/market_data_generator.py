from models.financial_atlas_models import Company, MarketData, MarketSnapshot
import datetime
from ai.llm_client import execute_prompt

def generate(company: Company) -> MarketData:
    """
    Uses AI to generate realistic market data (stock prices, market cap, volume).
    """

    prompt = f"""
    Generate realistic stock market data for **{company.name} (Ticker: {company.ticker})**.
    
    Include:
    - Current stock price
    - Market capitalization
    - Average trading volume
    - 30-day price history with realistic fluctuations.

    Output in valid JSON:
    ```json
    {{
        "current_price": "float",
        "market_cap": "float (in billions)",
        "volume": "integer",
        "price_history": [
            {{"date": "YYYY-MM-DD", "price": "float", "volume": "integer"}}
        ]
    }}
    ```
    """

    market_data = execute_prompt(prompt)

    # Convert price history into MarketSnapshot objects
    history = [
        MarketSnapshot(
            date=datetime.datetime.strptime(day["date"], "%Y-%m-%d"),
            price=day["price"],
            volume=day["volume"],
            market_cap=market_data["market_cap"]
        )
        for day in market_data["price_history"]
    ]

    return MarketData(
        ticker_symbol=company.ticker,
        current=history[-1],  # Latest price snapshot
        history=history[:-1]  # Last 29 days
    )
