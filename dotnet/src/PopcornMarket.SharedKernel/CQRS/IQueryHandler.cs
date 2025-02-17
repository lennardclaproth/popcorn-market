/******************************************************************************
 * File:        IQueryHandler.cs
 * Author:      Lennard Claproth
 * Date:        04/10/2024
 * Description: This file is a wrapper around MediatR to provide CQRS in a
 *              more descriptive manner by constraining the possibilities
 *              of implementation.
 *              
 * Version:     1.0.0
 ******************************************************************************/

using MediatR;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.SharedKernel.CQRS;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
    new Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
