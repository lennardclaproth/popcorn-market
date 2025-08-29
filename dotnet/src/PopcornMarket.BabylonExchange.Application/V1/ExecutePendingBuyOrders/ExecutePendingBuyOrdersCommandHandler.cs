using PopcornMarket.BabylonExchange.Domain.Abstractions.Repositories;
using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.ExecutePendingBuyOrders;

internal sealed class ExecutePendingBuyOrdersCommandHandler : ICommandHandler<ExecutePendingBuyOrdersCommand>
{
    private readonly IBuyOrderRepository _buyOrderRepository;
    private readonly IOrderBookRepository _orderBookRepository;

    public ExecutePendingBuyOrdersCommandHandler(IBuyOrderRepository buyOrderRepository, IOrderBookRepository orderBookRepository)
    {
        _buyOrderRepository = buyOrderRepository;
        _orderBookRepository = orderBookRepository;
    }

    public Task<Result> Handle(ExecutePendingBuyOrdersCommand request, CancellationToken cancellationToken)
    {
        // var orderBooks = await _orderBookRepository.GetAllOrderBooksWithPendingBuyOrders();
        // var pendingOrders = _buyOrderRepository.GetGroupedPendingBuyOrders();
        throw new NotImplementedException();
    }
}
