using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.UseCases.GetTickers;

public sealed record GetTickersQuery : IQuery<IEnumerable<string>>
{
    
}
