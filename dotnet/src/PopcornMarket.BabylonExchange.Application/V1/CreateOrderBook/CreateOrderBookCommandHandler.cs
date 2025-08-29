using Ardalis.GuardClauses;
using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.BabylonExchange.Domain.Entities;
using PopcornMarket.BabylonExchange.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.CreateOrderBook;

internal sealed class CreateOrderBookCommandHandler : ICommandHandler<CreateOrderBookCommand>
{
    private readonly IOrderBookRepository _orderBookRepository;
    private readonly ICompanyRepository _companyRepository;

    public CreateOrderBookCommandHandler(IOrderBookRepository orderBookRepository, ICompanyRepository companyRepository)
    {
        _orderBookRepository = orderBookRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Result> Handle(CreateOrderBookCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByTicker(request.Ticker);
        if (company == null) return Result.Failure(CompanyErrors.CompanyNotFound);
        
        var orderBook = await _orderBookRepository.GetByTicker(request.Ticker);
        if (orderBook != null) return Result.Failure(OrderBookErrors.OrderBookAlreadyExists);
        
        var creationResult = OrderBook.Create(request.Ticker);
        
        if(creationResult.IsFailure) return creationResult;
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value));

        await _orderBookRepository.AddEntity(creationResult.Value);
        
        return Result.Success();
    }
}
