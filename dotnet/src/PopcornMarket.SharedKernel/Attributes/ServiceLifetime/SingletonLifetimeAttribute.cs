namespace PopcornMarket.SharedKernel.Attributes.ServiceLifetime;

/// <summary>
/// Specifies that a single instance of the 'service' will be created.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SingletonLifetimeAttribute : Attribute
{
}
