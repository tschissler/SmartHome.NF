using ChargingControlController;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

var chargingController = new ChargingController();
TimeSpan readDeviceDataInterval = TimeSpan.FromSeconds(1);
TimeSpan updateChargingCurrencyInterval = TimeSpan.FromSeconds(2);


var refreshDataTimer = new Timer(new TimerCallback(chargingController.CalculateData), null, 0, (int)readDeviceDataInterval.TotalMilliseconds);
var updateChargingCurrencyTimer = new Timer(new TimerCallback(chargingController.SetChargingCurrency), null, 0, (int)updateChargingCurrencyInterval.TotalMilliseconds);

app.MapGet("/readchargingdata", string () =>
{
    lock (chargingController)
    {
        return JsonConvert.SerializeObject(chargingController.DataPoints);
    }
})
.WithName("ReadChargingData");

app.MapPost("/applychargingsettings", bool ([FromBody] ChargingSettingsData chargingSettingsData) =>
{
    chargingController.DataPoints.AutomaticCharging.CurrentValue = chargingSettingsData.automaticCharging;
    chargingController.DataPoints.MinimumPVShare.CurrentValue = chargingSettingsData.minimumPVShare;
    chargingController.DataPoints.ManualChargingCurrency.CurrentValue = chargingSettingsData.manualCurrency;
    return true;
})
.WithName("ApplyChargingSettings");

app.Run();