using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.GetTickers;

public sealed record GetTickersQuery : IQuery<IEnumerable<string>>
{
    
}
