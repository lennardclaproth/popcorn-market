using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialAtlas.Application.V1.CreateCompany;

internal sealed record CreateCompanyCommand : ICommand
{
    public required string Ticker { get; init; }
    public required string Name { get; init; }
    public required string Industry { get; init; }
    public required string Description { get; init; }
    public required string Headquarters { get; init; }
    public required string Ceo { get; init; }
    public required int FoundedYear { get; init; }
    public required int Employees { get; init; }
}
