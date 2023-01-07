using Microsoft.AspNetCore.Components.WebView.Maui;
using Newtonsoft.Json;
using Smarthome.App.Data;
using Syncfusion.Blazor;
using System.Reflection;

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
            try
            {
                //using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
                //using var reader = new StreamReader(stream);
                //var json = reader.ReadToEndAsync().Result;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Smarthome.App.appsettings.json";
                string json;

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
                var settings = JsonConvert.DeserializeObject<Settings>(json);

                // Add Syncfusion license
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(settings.SyncfusionLicenseKey);
            }
            catch (Exception ex)
            {
            }
            
            // Set IgnoreScriptIsolation as true to load scripts externally.
            builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });

            return builder.Build();
        }
    }
}