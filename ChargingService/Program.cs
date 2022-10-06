using ChargingService;
using KebaLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedContracts;
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

var chargingController = new ChargingController();
//var kebaConnector = new KebaDeviceConnector(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090, 1002);

//// According to the Keba documentation no other command should be sent for 2 seconds to ensure an undisturbed execution of the disable command.
//TimeSpan refreshDeviceDataInterval = TimeSpan.FromSeconds(2);

//var refreshDataTimer = new Timer(new TimerCallback(kebaConnector.RefreshData), null, 0, (int)refreshDeviceDataInterval.TotalMilliseconds);


app.MapGet("/readdata", string () =>
{
    return JsonConvert.SerializeObject(chargingController.GetDataPoints());
})
.WithName("ReadData");

app.MapGet("/readchargingsettings", string () =>
{
    return JsonConvert.SerializeObject(chargingController.GetChargingSettings());
})
.WithName("ReadChargingSettings");

app.MapPost("/setchargingcurrency", bool ([FromBody] double chargingCurrency) =>
{
    chargingController.SetChargingCurrency(chargingCurrency);
    return true;
})
.WithName("SetChargingCurrency");

app.MapPost("/applychargingsettings", bool ([FromBody] ChargingSettingsData chargingSettingsData) =>
{
    chargingController.ApplyChargingSettings(chargingSettingsData);
    return true;
})
.WithName("ApplyChargingSettings");

app.Run();