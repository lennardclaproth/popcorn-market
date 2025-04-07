using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace PopcornMarket.FinancialTimes.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection InstallApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddAutoMapper(assembly);

        return services;
    }
}
