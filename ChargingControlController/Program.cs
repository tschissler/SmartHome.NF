using ChargingControlController;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

var chargingController = new ChargingController();
TimeSpan readDeviceDataInterval = TimeSpan.FromSeconds(1);

var refreshDataTimer = new Timer(new TimerCallback(chargingController.CalculateData), null, 0, (int)readDeviceDataInterval.TotalMilliseconds);

app.MapGet("/readchargingdata", string () =>
{
    lock (chargingController)
    {
        return JsonConvert.SerializeObject(chargingController.DataPoints);
    }
})
.WithName("ReadChargingData");

app.MapPost("/applychargingsettings", bool ([FromBody] bool automaticCharging, double minimumPVShare, double manualCurrency) =>
{
    chargingController.DataPoints.AutomaticCharging.CurrentValue = automaticCharging;
    chargingController.DataPoints.MinimumPVShare.CurrentValue = minimumPVShare;
    chargingController.DataPoints.ManualChargingCurrency.CurrentValue = manualCurrency;
    return true;
})
.WithName("ApplyChargingSettings");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}