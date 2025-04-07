// using FastEndpoints;
// using MediatR;
// using PopcornMarket.FinancialTimes.Api.Extensions;
// using PopcornMarket.FinancialTimes.Application.V1.GetMacroEconomicArticles;
// using PopcornMarket.FinancialTimes.Contracts.V1.Dtos;
// using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
//
// namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetMacroEconomicArticles;
//
// public class GetMacroEconomicArticlesEndpoint : Endpoint<GetMacroEconomicArticlesRequest, IResult>
// {
//     private readonly ISender _sender;
//
//     public GetMacroEconomicArticlesEndpoint(ISender sender)
//     {
//         _sender = sender;
//     }
//
//     public override void Configure()
//     {
//         Get("/api/v1/articles/macro-economic");
//         AllowAnonymous();
//         Description(b => b
//             .Produces<IEnumerable<ArticleDto>>(200, "application/json")
//         );
//     }
//
//     public override async Task<IResult> ExecuteAsync(GetMacroEconomicArticlesRequest req, CancellationToken ct)
//     {
//         var query = new GetMacroEconomicArticlesQuery { Limit = req.Limit };
//         var result = await _sender.Send(query, ct);
//
//         return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
//     }
// }
