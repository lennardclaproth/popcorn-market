using MediatR;
using PopcornMarket.FinancialAtlas.Api.Abstractions;
using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application.V1.CreateCompany;
using PopcornMarket.FinancialAtlas.Contracts.Requests;

namespace PopcornMarket.FinancialAtlas.Api.Endpoints.V1.CreateCompany;

internal sealed class CreateCompanyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/company", async (
            CreateCompanyRequest req,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateCompanyCommand
            {
                Name = req.Name,
                Description = req.Description,
                Ceo = req.Ceo,
                Ticker = req.Ticker,
                Industry = req.Industry,
                FoundedYear = req.FoundedYear,
                Employees = req.Employees,
                Headquarters = req.Headquarters,
                Region = req.Region
            };

            var result = await sender.Send(command, ct);

            return result.IsFailure ? result.ToProblemDetails() : Results.Created();
        }).AllowAnonymous()
        .AddValidation<CreateCompanyRequest>();
    }
}
