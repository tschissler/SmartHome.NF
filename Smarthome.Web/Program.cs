using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Secrets;
using Smarthome.Web.Components;
using Smarthome.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
//builder.Services.AddSingleton<PowerDogDeviceConnector>();
//builder.Services.AddSingleton<DataPoints>();

var app = builder.Build();

//var powerDogDeviceConnector = app.Services.GetRequiredService<PowerDogDeviceConnector>();
//powerDogDeviceConnector.InitializePowerDogDeviceConnector(new UriBuilder("http", "192.168.178.150", 20000).Uri, PowerDogSecrets.Password, TimeSpan.FromSeconds(1));
//var dataPoints = app.Services.GetRequiredService<DataPoints>();
//dataPoints.InitializeDataPoints(powerDogDeviceConnector);

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
