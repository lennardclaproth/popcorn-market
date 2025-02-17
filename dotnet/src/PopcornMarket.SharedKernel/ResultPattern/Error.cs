/******************************************************************************
 * File:        Error.cs
 * Author:      Lennard Claproth
 * Date:        04/10/2024
 * Description: This file contains the Error record. This is a base
 *              implementation of an error, this serves as a base for creating
 *              errors. The goal of this is to have a clear description of
 *              when Domain errors occur.
 *              
 * Usage:
 *              public static readonly Error UserAlreadyExists = new Error(
 *                  "User.Exists",
 *                  "You cannot create a user that already exists");
 *              
 * Version:     1.0.0
 ******************************************************************************/

namespace PopcornMarket.SharedKernel.ResultPattern;

#pragma warning disable CA1716
/// <summary>
/// A class to help with simplifying error returns from domain models or commandHandlers;
/// </summary>
public sealed record Error
#pragma warning restore CA1716
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

    private Error(string code, string description, ErrorType errorType)
    {
        Code = code;
        Description = description;
        Type = errorType;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    /// <summary>
    /// Results in a NotFound error.
    /// </summary>
    /// <param name="code">The unique code of the error.</param>
    /// <param name="description">The description of the error.</param>
    /// <returns></returns>
    /// <example>
    /// <code>Error UserNotFound = Error.NotFound("IdPTenant.UserNotFound", "The user was not found under this tenant");</code>
    /// </example>
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>
    /// Results in a Validation error.
    /// </summary>
    /// <param name="code">The unique code of the error.</param>
    /// <param name="description">The description of the error.</param>
    /// <returns></returns>
    /// <example>
    /// <code>Error UserInvalidEmail = Error.Validation("User.InvalidEmail", "The email supplied was in an invalid format");</code>
    /// </example>
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    /// <summary>
    /// Results in a Conflict error.
    /// </summary>
    /// <param name="code">The unique code of the error.</param>
    /// <param name="description">The description of the error.</param>
    /// <returns></returns>
    /// <example>
    /// <code>Error UserAlreadyExists = Error.Conflict("IdPTenant.UserAlreadyExists", "The user already exists under this tenant");</code>
    /// </example>
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>
    /// Results in a Failure error.
    /// </summary>
    /// <param name="code">The unique code of the error.</param>
    /// <param name="description">The description of the error.</param>
    /// <returns></returns>
    /// <example>
    /// <code>Error CreationFailed = Error.Failure("IdPTenant.CreationFailed", "The creation failed due to invalid permissions");</code>
    /// </example>
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3
}
