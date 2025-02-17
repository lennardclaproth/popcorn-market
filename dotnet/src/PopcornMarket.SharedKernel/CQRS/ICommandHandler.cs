/******************************************************************************
 * File:        ICommandHandler.cs
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

/// <summary>
/// Defines a handler for processing a command and returning a response.
/// </summary>
/// <typeparam name="TCommand">
/// The type of the command to handle. Must implement <see cref="ICommand"/>.
/// </typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{

}

/// <summary>
/// Defines a handler for processing a command and returning a response.
/// </summary>
/// <typeparam name="TCommand">
/// The type of the command to handle. Must implement <see cref="ICommand{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned after processing the command.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{

}
