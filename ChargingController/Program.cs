using KebaLib;
using Newtonsoft.Json;
using System.Net;

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

var kebaConnector = new KebaDeviceConnector(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
TimeSpan readDeviceDataInterval = TimeSpan.FromSeconds(1);

var refreshDataTimer = new Timer(new TimerCallback(kebaConnector.RefreshData), null, 0, (int)readDeviceDataInterval.TotalMilliseconds);


app.MapGet("/readdata", string () =>
{
    return JsonConvert.SerializeObject(kebaConnector.DataPoints);
})
.WithName("ReadData");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}