using PopcornMarket.SharedKernel.CQRS;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.BabylonExchange.Application.V1.SetReferencePrice;

internal sealed class SetReferencePriceCommandHandler : ICommandHandler<SetReferencePriceCommand>
{
    public Task<Result> Handle(SetReferencePriceCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
