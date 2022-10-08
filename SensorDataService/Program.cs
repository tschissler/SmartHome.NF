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

app.MapGet("/readremotedisplaydata", string () =>
{
    lock (controller.LockObject)
    {
        return JsonConvert.SerializeObject(controller.remoteDisplayDataPoints);
    }
})
.WithName("ReadRemotedisplayData");

app.Run();
