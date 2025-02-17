using MediatR;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.SharedKernel.CQRS;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
