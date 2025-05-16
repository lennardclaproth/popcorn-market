using PopcornMarket.FinancialTimes.Domain.Abstractions;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.FinancialTimes.Application.V1.GetTickers;

internal sealed class GetTickersQueryHandler : IQueryHandler<GetTickersQuery, IEnumerable<string>>
{
    private readonly ICompanyRepository _repository;

    public GetTickersQueryHandler(ICompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<string>>> Handle(GetTickersQuery request, CancellationToken cancellationToken)
    {
        var tickers = await _repository.GetTickers();

        return Result<IEnumerable<string>>.Success(tickers);
    }
}
