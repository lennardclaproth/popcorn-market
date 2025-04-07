using PopcornMarket.FinancialTimes.Api.Extensions;
using PopcornMarket.FinancialTimes.Application;
using PopcornMarket.FinancialTimes.Application.Extensions;
using PopcornMarket.FinancialTimes.Persistence.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InstallPersistence(builder.Configuration["Persistence:ConnectionString"] ?? throw new InvalidOperationException());
builder.Services.InstallApplication(ApplicationAssemblyReference.Assembly);
builder.InstallPresentation();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.ConfigureEndpoints();
app.Run();
