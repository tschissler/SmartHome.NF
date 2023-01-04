using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedContracts.RestDataPoints;
using StorageService;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/addpvm3data", bool ([FromBody] List<PVM3RestDataPoint> pvM3RestDataPoints) =>
{
    StorageConnector.AddPVM3Data(pvM3RestDataPoints);
    return true;
})
.WithName("AddPVM3Data");

app.MapGet("/readpvm3data", string ([FromBody] DateTime timeStampFrom) =>
{
    if ((DateTime.Now.ToUniversalTime() - timeStampFrom).TotalSeconds > 600)
    {
        throw new InvalidOperationException("Time difference between now and timestamp is too big, maximum of 600 seconds is allowed");
    }
    try
    {
        return JsonConvert.SerializeObject(StorageConnector.ReadPVM3Data(timeStampFrom));
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException(ex.InnerException.Message);
    }
})
.WithName("ReadPVM3Data");

app.Run();
