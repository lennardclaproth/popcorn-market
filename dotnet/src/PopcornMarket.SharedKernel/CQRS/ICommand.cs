/******************************************************************************
 * File:        ICommandHandler.cs
 * Author:      Lennard Claproth
 * Date:        04/10/2024
 * Description: This file is a wrapper around MediatR IRequest to provide CQRS
 *              to provide CQRS in a more descriptive manner by constraining  
 *              the possibilities of implementation.
 * Version:     1.0.0
 ******************************************************************************/

using MediatR;
using PopcornMarket.SharedKernel.ResultPattern;

namespace PopcornMarket.SharedKernel.CQRS;

/// <summary>
/// Defines a command to be used in the <see cref="ICommandHandler{TCommand}"/>.
/// This is a wrapper implementation of the MediatR <see cref="IRequest"/>
/// </summary>
public interface ICommand : IRequest<Result>
{

}

/// <summary>
/// Defines a command to be used in the <see cref="ICommandHandler{TCommand,TResponse}"/>.
/// This is a wrapper implementation of the MediatR <see cref="IRequest{TResponse}"/>
/// <typeparam name="TResponse">The response type of the command.</typeparam>
/// </summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{

}
