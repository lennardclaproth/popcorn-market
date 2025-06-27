from pydantic import BaseModel, Field
from typing import List, Optional
from datetime import datetime

class Company(BaseModel):
    """Represents a publicly traded company."""
    ticker: str = Field(..., title="Stock Ticker Symbol")
    name: str = Field(..., title="Company Name")
    industry: str = Field(..., title="Industry Sector")
    description: str = Field(..., title="Short company description")
    headquarters: str = Field(..., title="Company Headquarters")
    ceo: str = Field(..., title="CEO Name")
    founded_year: int = Field(..., title="Year the company was founded")
    employees: int = Field(..., title="Total number of employees")
    region: str = Field(..., title="The region of the company")

class MarketSnapshot(BaseModel):
    """Represents a snapshot of a stock's market performance."""
    stock_price_usd: float = Field(..., title="Stock Price in USD", alias="stock_price_USD")
    volume: int = Field(..., title="Volume of the stocks traded")
    market_cap_b: float = Field(..., title="Market Capitalization in Billions", alias="market_cap_B")
    dividend_per_share_usd: Optional[float] = Field(default=None, title="Dividend per Share in USD")
    dividend_yield_percent: Optional[float] = Field(default=None, title="Dividend Yield Percentage")
    date: datetime = Field(..., title="Snapshot Date")


class MarketData(BaseModel):
    """Represents the market data for a company, including historical prices."""
    ticker: str = Field(..., title="Stock Ticker Symbol")
    shares_outstanding: int = Field(..., title="Outstanding shares of the company,")
    current: MarketSnapshot = Field(..., title="Current Market Data Snapshot")
    history: List[MarketSnapshot] = Field(default=[], title="Historical Market Data")

class IncomeStatement(BaseModel):
    """Represents a company's income statement."""
    revenue_b: float = Field(..., title="Revenue in Billions")
    cogs_b: float = Field(..., title="Cost of Goods Sold in Billions")
    gross_profit_b: float = Field(..., title="Gross Profit in Billions")
    operating_expenses_b: float = Field(..., title="Operating Expenses in Billions")
    ebitda_b: float = Field(..., title="EBITDA in Billions")
    depreciation_amortization_b: float = Field(..., title="Depreciation & Amortization in Billions")
    ebit_b: float = Field(..., title="EBIT in Billions")
    interest_expense_b: float = Field(..., title="Interest Expenses in Billions")
    tax_rate_percent: float = Field(..., title="Tax Rate Percentage")
    net_income_b: float = Field(..., title="Net Income in Billions")
    eps_usd: float = Field(..., title="Earnings per Share (USD)")


class BalanceSheet(BaseModel):
    """Represents a company's balance sheet."""
    total_assets_b: float = Field(..., title="Total Assets in Billions")
    total_liabilities_b: float = Field(..., title="Total Liabilities in Billions")
    total_equity_b: float = Field(..., title="Total Equity in Billions")
    debt_to_equity_ratio: float = Field(..., title="Debt-to-Equity Ratio")


class CashFlowStatement(BaseModel):
    """Represents a company's cash flow statement."""
    operating_cash_flow_b: float = Field(..., title="Operating Cash Flow in Billions")
    capital_expenditures_b: float = Field(..., title="Capital Expenditures in Billions")
    free_cash_flow_b: float = Field(..., title="Free Cash Flow in Billions")
    financing_cash_flow_b: float = Field(..., title="Financing Cash Flow in Billions")
    investing_cash_flow_b: float = Field(..., title="Investing Cash Flow in Billions")
    net_cash_flow_b: float = Field(..., title="Net Cash Flow in Billions")


class FinancialStatement(BaseModel):
    """Represents a company's financial statement for a specific period."""
    ticker_symbol: str = Field(..., title="Stock Ticker Symbol")
    year: int = Field(..., title="Year")
    interval: str = Field(..., title="Financial Period (e.g., Q1, Annual)")
    income_statement: IncomeStatement
    balance_sheet: BalanceSheet
    cash_flow_statement: CashFlowStatement
