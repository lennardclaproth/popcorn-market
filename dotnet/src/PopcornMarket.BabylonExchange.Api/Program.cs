using PopcornMarket.BabylonExchange.Api.Extensions;
using PopcornMarket.BabylonExchange.Application.Extensions;
using PopcornMarket.BabylonExchange.Infrastructure.Extensions;
using PopcornMarket.BabylonExchange.Persistence.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InstallPersistence(builder.Configuration["Persistence:ConnectionString"] ?? throw new InvalidOperationException());
builder.Services.InstallInfrastructure(builder.Configuration);
builder.Services.InstallApplication();
builder.Services.AddAllElasticApm();
builder.InstallPresentation();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.ConfigureEndpoints();
app.Run();
