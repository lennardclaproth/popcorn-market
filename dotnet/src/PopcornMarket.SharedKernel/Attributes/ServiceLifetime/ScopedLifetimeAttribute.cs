namespace PopcornMarket.SharedKernel.Attributes.ServiceLifetime;

/// <summary>
/// Specifies that a new instance of the 'service' will be created for each scope.
/// In API apps, a scope is created around each server request.
/// In Azure functions, a scope is created for each function trigger/request.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ScopedLifetimeAttribute : Attribute
{
}
