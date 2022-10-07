using Microsoft.AspNetCore.Mvc;
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

app.MapGet("/gettremotedisplaydata", RemoteDisplayDataPoints () =>
{
    return controller.remoteDisplayDataPoints;
})
.WithName("SetRemotedisplayData");

app.Run();
