using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.PublishAnalysis;
public sealed record PublishAnalysisCommand : ICommand
{
    public string Ticker { get; init; } = null!;
    public float Current {get; init;}
    public float OneWeek { get; init; }
    public float OneMonth { get; init; }
    public float ThreeMonths { get; init; }
    public decimal TargetPrice { get; init; }
    public DateTime Date { get; init; }
}
