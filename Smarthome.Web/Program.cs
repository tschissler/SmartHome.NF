using Keba;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Secrets;
using Smarthome.Web.Components;
using Smarthome.Web.Controllers;
using Smarthome.Web.Data;
using Syncfusion.Blazor;
using System.Net;

Console.Clear();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5000;https://*:5001");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });

PowerDogDeviceConnector powerDogDeviceConnector = new(new UriBuilder("http", "192.168.178.150", 20000).Uri, PowerDogSecrets.Password, TimeSpan.FromSeconds(1));
KebaDeviceConnector kebaDeviceConnector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090, TimeSpan.FromSeconds(1));
ChargingController chargingController = new(powerDogDeviceConnector, kebaDeviceConnector);
DataPoints DataPoints = new DataPoints(powerDogDeviceConnector, kebaDeviceConnector, chargingController);

builder.Services.AddSingleton(powerDogDeviceConnector);
builder.Services.AddSingleton(kebaDeviceConnector);
builder.Services.AddSingleton(chargingController);
builder.Services.AddSingleton(DataPoints);

var app = builder.Build();

//var powerDogDeviceConnector = app.Services.GetRequiredService<PowerDogDeviceConnector>();
//powerDogDeviceConnector.InitializePowerDogDeviceConnector(new UriBuilder("http", "192.168.178.150", 20000).Uri, PowerDogSecrets.Password, TimeSpan.FromSeconds(1));
//var kebaDeviceConnector = app.Services.GetRequiredService<KebaDeviceConnector>();
//kebaDeviceConnector.InitializeKebaDeviceConnector(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090, TimeSpan.FromSeconds(1));
//var dataPoints = app.Services.GetRequiredService<DataPoints>();
//dataPoints.InitializeDataPoints(powerDogDeviceConnector, kebaDeviceConnector);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
