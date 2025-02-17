using Microsoft.Extensions.DependencyInjection;

namespace PopcornMarket.FinancialAtlas.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection InstallApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssemblyReference.Assembly));
        return services;
    }
}
