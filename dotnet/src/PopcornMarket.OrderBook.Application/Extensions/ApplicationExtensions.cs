using Microsoft.Extensions.DependencyInjection;

namespace PopcornMarket.OrderBook.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection InstallApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssemblyReference.Assembly));
        services.AddAutoMapper(ApplicationAssemblyReference.Assembly);

        return services;
    }
}
