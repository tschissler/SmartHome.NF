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

app.MapGet("/readdata", string () =>
{
    lock (chargingController.lockobject)
    {
        return JsonConvert.SerializeObject(chargingController.GetDataPoints());
    }
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