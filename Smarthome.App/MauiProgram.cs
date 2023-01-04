using HelpersLib;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Smarthome.App.Data;
using Syncfusion.Blazor;

namespace Smarthome.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
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

            builder.Services.AddSingleton<WeatherForecastService>();
            
            // Set IgnoreScriptIsolation as true to load scripts externally.
            builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            
            return builder.Build();
        }
    }
}