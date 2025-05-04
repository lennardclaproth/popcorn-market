using PopcornMarket.FinancialAtlas.Api.Extensions;
using PopcornMarket.FinancialAtlas.Application;
using PopcornMarket.FinancialAtlas.Application.Extensions;
using PopcornMarket.FinancialAtlas.Infrastructure.Extensions;
using PopcornMarket.FinancialAtlas.Persistence.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InstallPersistence(builder.Configuration["Persistence:ConnectionString"] ?? throw new InvalidOperationException());
builder.Services.InstallApplication(ApplicationAssemblyReference.Assembly);
builder.Services.InstallInfrastructure();

builder.InstallPresentation();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.ConfigureEndpoints();
app.Run();

