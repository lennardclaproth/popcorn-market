using Serilog;

namespace PopcornMarket.FinancialAtlas.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.logging.json", optional: false, reloadOnChange: true);

        builder.Host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(context.Configuration);
        });
        
        return builder;
    }
}
