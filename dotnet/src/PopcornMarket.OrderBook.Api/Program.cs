using PopcornMarket.OrderBook.Api.Extensions;
using PopcornMarket.OrderBook.Application.Extensions;
using PopcornMarket.OrderBook.Infrastructure.Extensions;
using PopcornMarket.OrderBook.Persistence.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InstallPersistence(builder.Configuration["Persistence:ConnectionString"] ?? throw new InvalidOperationException());
builder.Services.InstallApplication();
builder.InstallPresentation();
builder.Services.InstallInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.ConfigureEndpoints();
app.Run();
