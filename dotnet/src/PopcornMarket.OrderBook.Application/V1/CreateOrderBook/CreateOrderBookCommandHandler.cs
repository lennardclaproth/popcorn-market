using Ardalis.GuardClauses;
using PopcornMarket.OrderBook.Domain.Abstractions.Repositories;
using PopcornMarket.OrderBook.Domain.Errors;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.OrderBook.Application.V1.CreateOrderBook;

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
        
        var creationResult = Domain.Entities.OrderBook.Create(request.Ticker);
        
        if(creationResult.IsFailure) return creationResult;
        
        Guard.Against.Null(creationResult.Value, nameof(creationResult.Value));

        await _orderBookRepository.AddEntity(creationResult.Value);
        
        return Result.Success();
    }
}
