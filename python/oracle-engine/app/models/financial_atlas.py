import re
from pydantic import BaseModel, ConfigDict, Field
from typing import List, Optional
from datetime import datetime
from enum import IntEnum

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
    dividend_per_share_usd: Optional[float] = Field(default=None, title="Dividend per Share in USD", alias="dividend_per_share_USD")
    dividend_yield_percent: Optional[float] = Field(default=None, title="Dividend Yield Percentage")
    date: datetime = Field(..., title="Snapshot Date")
    model_config = ConfigDict(
        populate_by_name=True,
        extra="forbid"
    )


class MarketData(BaseModel):
    """Represents the market data for a company, including historical prices."""
    ticker: str = Field(..., title="Stock Ticker Symbol")
    shares_outstanding: int = Field(..., title="Outstanding shares of the company,")
    current: MarketSnapshot = Field(..., title="Current Market Data Snapshot")
    history: Optional[List[MarketSnapshot]] = Field(default=None, title="Historical Market Data")


class IncomeStatement(BaseModel):
    """Represents a company's income statement."""
    revenue_b: float = Field(..., title="Revenue in Billions", alias="revenue_B")
    cogs_b: float = Field(..., title="Cost of Goods Sold in Billions", alias="cogs_B")
    gross_profit_b: float = Field(..., title="Gross Profit in Billions", alias="gross_profit_B")
    operating_expenses_b: float = Field(..., title="Operating Expenses in Billions", alias="operating_expenses_B")
    ebitda_b: float = Field(..., title="EBITDA in Billions", alias="ebitda_B")
    depreciation_amortization_b: float = Field(..., title="Depreciation & Amortization in Billions", alias="depreciation_amortization_B")
    ebit_b: float = Field(..., title="EBIT in Billions", alias="ebit_B")
    interest_expense_b: float = Field(..., title="Interest Expenses in Billions", alias="interest_expense_B")
    tax_rate_percent: float = Field(..., title="Tax Rate Percentage", alias="tax_rate_percent")
    net_income_b: float = Field(..., title="Net Income in Billions", alias="net_income_B")
    eps_usd: float = Field(..., title="Earnings per Share (USD)", alias="eps_USD")
    model_config = ConfigDict(
        populate_by_name=True,
        extra="forbid"
    )


class BalanceSheet(BaseModel):
    """Represents a company's balance sheet."""
    total_assets_b: float = Field(..., title="Total Assets in Billions", alias="total_assets_B")
    total_liabilities_b: float = Field(..., title="Total Liabilities in Billions", alias="total_liabilities_B")
    total_equity_b: float = Field(..., title="Total Equity in Billions", alias="total_equity_B")
    debt_to_equity_ratio: float = Field(..., title="Debt-to-Equity Ratio")
    model_config = ConfigDict(
        populate_by_name=True,
        extra="forbid"
    )


class CashFlowStatement(BaseModel):
    """Represents a company's cash flow statement."""
    operating_cash_flow_b: float = Field(..., title="Operating Cash Flow in Billions", alias="operating_cash_flow_B")
    capital_expenditures_b: float = Field(..., title="Capital Expenditures in Billions", alias="capital_expenditures_B")
    free_cash_flow_b: float = Field(..., title="Free Cash Flow in Billions", alias="free_cash_flow_B")
    financing_cash_flow_b: float = Field(..., title="Financing Cash Flow in Billions", alias="financing_cash_flow_B")
    investing_cash_flow_b: float = Field(..., title="Investing Cash Flow in Billions", alias="investing_cash_flow_B")
    net_cash_flow_b: float = Field(..., title="Net Cash Flow in Billions", alias="net_cash_flow_B")
    model_config = ConfigDict(
        populate_by_name=True,
        extra="forbid"
    )


class FinancialStatement(BaseModel):
    """Represents a company's financial statement for a specific period."""
    ticker: str = Field(..., title="Stock Ticker Symbol")
    period: str = Field(..., title="Financial Period (e.g., Q1 2023)")
    publish_date: datetime = Field(..., title="Date of Publication")
    income_statement: IncomeStatement
    balance_sheet: BalanceSheet
    cash_flow_statement: CashFlowStatement

class PeriodType(IntEnum):
    Quarterly = 0
    HalfYearly = 1
    Yearly = 2

class PublishFinancialStatementRequest(BaseModel):
    """Request model for publishing a financial statement."""
    ticker: str = Field(..., title="Stock Ticker Symbol")
    year: int
    interval: PeriodType
    period_number: int = Field(..., alias="period_number")
    income_statement: IncomeStatement
    balance_sheet: BalanceSheet
    cash_flow_statement: CashFlowStatement
    model_config = ConfigDict(
        populate_by_name=True,
        extra="forbid"
    )

class ReportingPeriod(BaseModel):
    year: int
    type: PeriodType
    period_number: int

    @classmethod
    def next_period(cls, current: "ReportingPeriod") -> "ReportingPeriod":
        if current.type == PeriodType.Yearly:
            return ReportingPeriod(year=current.year + 1, type=PeriodType.Yearly, period_number=1)
        elif current.type == PeriodType.HalfYearly:
            if current.period_number == 1:
                return ReportingPeriod(year=current.year, type=PeriodType.HalfYearly, period_number=2)
            else:
                return ReportingPeriod(year=current.year + 1, type=PeriodType.Yearly, period_number=1)
        elif current.type == PeriodType.Quarterly:
            if current.period_number < 4:
                return ReportingPeriod(year=current.year, type=PeriodType.Quarterly, period_number=current.period_number + 1)
            else:
                return ReportingPeriod(year=current.year + 1, type=PeriodType.Yearly, period_number=1)
        else:
            raise ValueError("Invalid ReportingPeriod type")

    @classmethod
    def from_string(cls, period_str: str) -> "ReportingPeriod":
        # Matches "2025 Q2" or "2024 H1" or "2023"
        q_match = re.match(r"^(\d{4}) Q([1-4])$", period_str)
        h_match = re.match(r"^(\d{4}) H([1-2])$", period_str)
        y_match = re.match(r"^(\d{4})$", period_str)

        if q_match:
            return cls(year=int(q_match[1]), type=PeriodType.Quarterly, period_number=int(q_match[2]))
        elif h_match:
            return cls(year=int(h_match[1]), type=PeriodType.HalfYearly, period_number=int(h_match[2]))
        elif y_match:
            return cls(year=int(y_match[1]), type=PeriodType.Yearly, period_number=1)
        else:
            raise ValueError(f"Invalid ReportingPeriod format: '{period_str}'")