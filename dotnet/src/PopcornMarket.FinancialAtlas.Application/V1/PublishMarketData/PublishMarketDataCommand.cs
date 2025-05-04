using PopcornMarket.FinancialAtlas.Contracts.Dtos;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.PublishMarketData;

public record PublishMarketDataCommand : ICommand
{
    public string Ticker { get; init; } = null!;
    public long SharesOutstanding { get; init; }
    public MarketSnapshotDto Current { get; init; } = null!;
    public List<MarketSnapshotDto> History { get; init; } = new();
}
