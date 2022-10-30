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
    lock (controller.LockObject)
    {
        return JsonConvert.SerializeObject(controller.remoteDisplayDataPoints);
    }
})
.WithName("ReadRemotedisplayData");

app.MapGet("/readconsumptiondata", string () =>
{
    lock (controller.LockObject)
    {
        return JsonConvert.SerializeObject(controller.consumptionDataPoints);
    }
})
.WithName("ReadConsumptionData");

app.MapGet("/ping", bool () =>
{
    return true;
})
.WithName("Ping");

app.Run();
