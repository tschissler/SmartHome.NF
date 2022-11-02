using HelpersLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensorDataService;
using SharedContracts;
using SharedContracts.DataPointCollections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

SensorsController controller = new ();

app.MapPost("/setremotedisplaydata", bool ([FromBody] RemoteDisplayData remotedisplaydata) =>
{
    controller.RemoteDisplayChanged(remotedisplaydata);
    return true;
})
.WithName("SetRemotedisplayData");

app.MapPost("/writeconsumptiondata", bool ([FromBody] ConsumptionData consumptiondata) =>
{
    controller.ConsumptionChanged(consumptiondata);
    return true;
})
.WithName("WriteConsumptionData");

app.MapGet("/readremotedisplaydata", string () =>
{
    if (Monitor.TryEnter(controller.LockObject, 1000))
    {
        try
        {
            return JsonConvert.SerializeObject(controller.remoteDisplayDataPoints);
        }
        catch (Exception ex)
        {
            ConsoleHelpers.PrintErrorMessage($"Error reading remote display data: {ex.Message}");
        }
        finally
        {
            Monitor.Exit(controller.LockObject);
        }
    }
    return "Locked";
})
.WithName("ReadRemotedisplayData");

app.MapGet("/readconsumptiondata", string () =>
{
    if (Monitor.TryEnter(controller.LockObject, 1000))
    {
        try
        {
            return JsonConvert.SerializeObject(controller.consumptionDataPoints);
        }
        catch (Exception ex)
        {
            ConsoleHelpers.PrintErrorMessage($"Error reading remote display data: {ex.Message}");
        }
        finally
        {
            Monitor.Exit(controller.LockObject);
        }
    }
    return "Locked";
})
.WithName("ReadConsumptionData");

app.MapGet("/ping", bool () =>
{
    return true;
})
.WithName("Ping");

app.Run();
