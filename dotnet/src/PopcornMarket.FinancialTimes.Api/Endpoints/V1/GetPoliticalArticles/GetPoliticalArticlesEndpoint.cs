// using FastEndpoints;
// using MediatR;
// using PopcornMarket.FinancialTimes.Api.Extensions;
// using PopcornMarket.FinancialTimes.Application.V1.GetPoliticalArticles;
// using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
//
// namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetPoliticalArticles;
//
// public class GetPoliticalArticlesEndpoint : Endpoint<GetPoliticalArticlesRequest, IResult>
// {
//     private readonly ISender _sender;
//
//     public GetPoliticalArticlesEndpoint(ISender sender)
//     {
//         _sender = sender;
//     }
//
//     public override void Configure()
//     {
//         Get("api/v1/articles/political");
//         AllowAnonymous();
//     }
//
//     public override async Task<IResult> ExecuteAsync(GetPoliticalArticlesRequest req, CancellationToken ct)
//     {
//         var query = new GetPoliticalArticlesQuery { Limit = req.Limit, };
//         
//         var result = await _sender.Send(query, ct);
//         
//         return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
//     }
// }
