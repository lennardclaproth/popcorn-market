using System.Text.Json.Serialization;

namespace PopcornMarket.FinancialAtlas.Contracts.Dtos;

public sealed record CashFlowStatementDto()
{
    [JsonPropertyName("operating_cash_flow_B")]
    public decimal OperatingCashFlowB { get; init; }
    
    [JsonPropertyName("capital_expenditures_B")]
    public decimal CapitalExpendituresB { get; init; }
    
    [JsonPropertyName("free_cash_flow_B")]
    public decimal FreeCashFlowB { get; init; }
    
    [JsonPropertyName("financing_cash_flow_B")]
    public decimal FinancingCashFlowB { get; init; }
    
    [JsonPropertyName("investing_cash_flow_B")]
    public decimal InvestingCashFlowB { get; init; }
    
    [JsonPropertyName("net_cash_flow_B")]
    public decimal NetCashFlowB { get; init; }
}
