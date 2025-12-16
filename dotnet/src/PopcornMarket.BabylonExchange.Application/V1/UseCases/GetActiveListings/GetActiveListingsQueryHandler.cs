using AutoMapper;
using Microsoft.Extensions.Logging;
using PopcornMarket.BabylonExchange.Contracts.Dtos;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.UseCases.GetActiveListings;
internal sealed class GetActiveListingsQueryHandler : IQueryHandler<GetActiveListingsQuery, IEnumerable<ListingDto>>
{
    private readonly IListingRepository _listingRepository;
    private readonly ITradeRepository _tradeRepository;
    private readonly IMapper _mapper;

    public GetActiveListingsQueryHandler(IListingRepository listingRepository, IMapper mapper, ILogger<GetActiveListingsQueryHandler> logger, ITradeRepository tradeRepository)
    {
        _listingRepository = listingRepository;
        _mapper = mapper;
        _tradeRepository = tradeRepository;
    }

    public async Task<Result<IEnumerable<ListingDto>>> Handle(GetActiveListingsQuery request, CancellationToken cancellationToken)
    {
        var dbListings = await _listingRepository.GetActiveListings(request.Filter, request.PageNumber, request.PageSize);

        var mappedListings = _mapper.Map<IEnumerable<ListingDto>>(dbListings);

        var listings = mappedListings.ToList();

        foreach (var listing in listings)
        {
            var trade = await _tradeRepository.GetLastExecutedTrade(listing.StockSymbol);
            listing.LastPrice = trade.Price;
        }

        return Result<IEnumerable<ListingDto>>.Success(listings);
    }
}
