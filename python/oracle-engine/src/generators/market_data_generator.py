import random
from models.financial_atlas_models import Company, MarketData, MarketSnapshot
from datetime import datetime, timedelta
from ai.llm_client import execute_prompt

def generate(company: Company) -> MarketData:
    """
    Uses AI to generate realistic market data (stock prices, market cap, volume).
    """
    current_price = round(random.uniform(5,500), 2)
    shares_outstanding = random.randint(10_000_000, 2_000_000_000)
    market_cap = round((current_price * shares_outstanding) / 1_000_000_000, 2)  # in billions
    avg_volume = random.randint(100_000, 10_000_000)

    current = MarketSnapshot(
        date=datetime.now(),
        stock_price_usd=current_price,
        volume=int(random.gauss(avg_volume, avg_volume * 0.1)),  # simulate volume variation
        market_cap_b = market_cap
    )

    history = generate_price_history(current_price, avg_volume, shares_outstanding, 365)
    return MarketData(
        ticker=company.ticker,
        shares_outstanding=shares_outstanding,
        current=current,
        history=history
    )

def generate_price_history(current_price, avg_volume, shares_outstanding ,days=30):
    """
    Simulates price history by using simple brownian Motion
    """
    history : list[MarketSnapshot] = []
    date = datetime.now()
    price = current_price

    for i in range(days):
        date -= timedelta(days=1)
        
        # Simulate slight movement: daily % change between -2% and +2%
        delta_pct = random.uniform(-0.02, 0.02)
        price = round(price * (1 + delta_pct), 2)
        volume = int(random.gauss(avg_volume, avg_volume * 0.1))  # simulate volume variation

        history.append(MarketSnapshot(
            date=date,
            stock_price_usd=price,
            volume=volume,
            market_cap_b = round((price * shares_outstanding) / 1_000_000_000, 2)
        ))

    history.reverse()  # so it's in chronological order
    return history

    # prompt = f"""
    # Generate realistic stock market data for **{company.name} (Ticker: {company.ticker})**.
    
    # Include:
    # - Current stock price
    # - Market capitalization
    # - Average trading volume
    # - 30-day price history with realistic fluctuations.

    # Output in valid JSON:
    # ```json
    # {{
    #     "current_price": "float",
    #     "market_cap": "float (in billions)",
    #     "volume": "integer",
    #     "shares_outstanding":"integer"
    #     "price_history": [
    #         {{"date": "YYYY-MM-DD", "price": "float", "volume": "integer"}}
    #     ]
    # }}
    # ```
    # """

    # market_data = execute_prompt(prompt)

    # # Convert price history into MarketSnapshot objects
    # history = [
    #     MarketSnapshot(
    #         date=datetime.datetime.strptime(day["date"], "%Y-%m-%d"),
    #         price=day["price"],
    #         volume=day["volume"],
    #         market_cap=market_data["market_cap"]
    #     )
    #     for day in market_data["price_history"]
    # ]

    # return MarketData(
    #     ticker_symbol=company.ticker,
    #     current=history[-1],  # Latest price snapshot
    #     history=history[:-1]  # Last 29 days
    # )
