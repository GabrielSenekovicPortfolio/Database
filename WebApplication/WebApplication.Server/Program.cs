using System;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Database;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddSingleton<SkuDatabase>(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<SkuDatabase>>();

    string csvPath = Path.Combine(Directory.GetCurrentDirectory(), "price_detail.csv");
    logger.LogInformation($"Looking for CSV at: {csvPath}");

    CSVReader.Read("price_detail.csv", out List<DatasetEntry> entriesList);
    DatabaseFactory.Create(entriesList, out SkuDatabase database);
    return database;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
