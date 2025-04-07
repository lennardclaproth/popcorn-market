using FluentValidation;

namespace PopcornMarket.FinancialTimes.Api.Extensions;

public static class PresentationExtensions
{
    public static WebApplicationBuilder InstallPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpoints(PresentationAssemblyReference.Assembly);
        builder.Services.AddValidatorsFromAssembly(PresentationAssemblyReference.Assembly);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.AddLogging();
        
        return builder;
    }
}
