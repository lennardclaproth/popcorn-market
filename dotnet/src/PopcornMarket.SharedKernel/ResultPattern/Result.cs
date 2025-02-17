/******************************************************************************
 * File:        Result.cs
 * Author:      Lennard Claproth
 * Date:        04/10/2024
 * Description: This file contains a Result class and a Result class with a
 *              value parameter. The goal of this class is to conform to the
 *              Result pattern. Instead of throwing exceptions when expected
 *              errors occur we can return generic Result objects. This is to
 *              improve API performance considering throwing exceptions is
 *              costly. This result pattern is also part of the CQRS
 *              implementation int this architecture.
 *              
 * Usage:
 *              return Result.Failure<TResult>(Domain.Errors.MyError)
 *              return Result.IsSuccess ?? Result.Value : Result.ToProblemDetails()
 *              
 * Version:     1.0.0
 ******************************************************************************/


using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS0108, CS0114

namespace PopcornMarket.SharedKernel.ResultPattern;

/// <summary>
/// Defines a Result class to standardize result handling.
/// </summary>
/// <typeparam name="TValue">The type of the value of the result.</typeparam>
/// <example>
/// Examples
/// <para>Example 1: Creating a success result</para>
/// <code>
/// var successResult = Result.Success();
/// </code>
/// <para>Example 2: Creating a failure result</para>
/// <code>
/// var failureResult = Result.Failure(Domain.Errors.MyError));
/// </code>
/// </example>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new Result(true, Error.None);
    public static Result Failure(Error error) => new Result(false, error);
}

/// <summary>
/// Defines a Result class to standardize result handling.
/// </summary>
/// <typeparam name="TValue">The type of the value of the result.</typeparam>
/// <example>
/// Examples
/// <para>Example 1: Creating a success result</para>
/// <code>
/// var successResult = <![CDATA[Result<string>]]>.Success("Operation completed successfully.");
/// </code>
/// <para>Example 2: Creating a failure result</para>
/// <code>
/// var failureResult = <![CDATA[Result<string>]]>.Failure(Domain.Errors.MyError);
/// </code>
/// </example>
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types")]
public class Result<TValue> : Result
{
    protected Result(bool isSuccess, Error error, TValue value)
        : base(isSuccess, error)
    {
        Value = value;
    }

    protected Result(bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = default;
    }

    public TValue? Value { get; }

    public static Result<TValue> Success(TValue value) => new Result<TValue>(true, Error.None, value);
    public static Result<TValue> Failure(Error error) => new Result<TValue>(false, error);
}
