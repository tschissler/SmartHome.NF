using HelpersLib;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5000;https://*:5001");

string SyncfusionLicenseKeyEnvironmentVariable = "SyncfusionLicenseKey";

if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable(SyncfusionLicenseKeyEnvironmentVariable)))
{
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(System.Environment.GetEnvironmentVariable(SyncfusionLicenseKeyEnvironmentVariable));
    ConsoleHelpers.PrintInformation("Setting license key for syncfusion components");
}
else
{
    ConsoleHelpers.PrintInformation($"Could not find environment variable {SyncfusionLicenseKeyEnvironmentVariable}, cannot set license key for Syncfusion lib.");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
