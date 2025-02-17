namespace PopcornMarket.SharedKernel.Attributes.ServiceLifetime;

/// <summary>
/// Specifies that a new instance of the 'service' will be created every time it is requested.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TransientLifetimeAttribute : Attribute
{
}
