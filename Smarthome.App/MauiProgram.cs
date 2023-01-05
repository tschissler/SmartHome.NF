using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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

            const string secretName = "SyncfusionLicenseKey";
            var keyVaultName = "SmartHomeKeyVault";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            var secret = client.GetSecretAsync(secretName).Result;


            //var clientId = "your-client-id";
            //var clientSecret = "your-client-secret";
            //var tenantId = "your-tenant-id";

            //var azureServiceTokenProvider = new AzureServiceTokenProvider();
            //var keyVaultClient = new KeyVaultClient(
            //    new KeyVaultClient.AuthenticationCallback(
            //        azureServiceTokenProvider.KeyVaultTokenCallback));


            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(secret.Value.Value);

            builder.Services.AddSingleton<WeatherForecastService>();
            
            // Set IgnoreScriptIsolation as true to load scripts externally.
            builder.Services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            
            return builder.Build();
        }
    }
}