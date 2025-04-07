// using FastEndpoints;
// using MediatR;
// using PopcornMarket.FinancialTimes.Api.Extensions;
// using PopcornMarket.FinancialTimes.Application.V1.GetArticlesByTicker;
// using PopcornMarket.FinancialTimes.Contracts.V1.Requests;
//
// namespace PopcornMarket.FinancialTimes.Api.Endpoints.V1.GetArticlesByTicker;
//
// public class GetArticlesByTickerEndpoint : Endpoint<GetArticlesByTickerRequest, IResult>
// {
//     private readonly ISender _sender;
//
//     public GetArticlesByTickerEndpoint(ISender sender)
//     {
//         _sender = sender;
//     }
//
//     public override void Configure()
//     {
//         Get("api/v1/articles/ticker/{ticker}");
//         AllowAnonymous();
//     }
//
//     public override async Task<IResult> ExecuteAsync(GetArticlesByTickerRequest req, CancellationToken ct)
//     {
//         var query = new GetArticlesByTickerQuery { Ticker = req.Ticker, Limit = req.Limit };
//         
//         var result = await _sender.Send(query, ct);
//         
//         return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblemDetails();
//     }
// }
