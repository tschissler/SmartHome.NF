using HelpersLib;
using Newtonsoft.Json;
using PowerDogLib;
using SharedContracts.DataPointCollections;

namespace PVService
{
    public class PVStorageConnector
    {
        private PowerDog powerDog;
        private const string storageServiceUrl = "http://localhost:5006";


        public PVStorageConnector(PowerDog powerDog)
        {
            this.powerDog = powerDog;
        }
        
        public void SendDataToCloud(object? state)
        {
            try
            {
                var data = powerDog.ReadCloudDataCache();

                using (HttpClient Http = new HttpClient())
                {
                    //var result = Http.PostAsJsonAsync($"https://smarthomestorageservice.azurewebsites.net/addpvm3data", data, new CancellationToken()).Result;
                    var result = Http.PostAsJsonAsync($"{storageServiceUrl}/addpvm3data", data, new CancellationToken()).Result;

                    if (result.EnsureSuccessStatusCode().IsSuccessStatusCode) 
                    {
                        ConsoleHelpers.PrintInformation($"Successfully submitted data to the cloud");
                    }
                    else
                    {
                        ConsoleHelpers.PrintErrorMessage($"Error while sending data to storage servive {storageServiceUrl} - Error Code: {result.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Error sending data to StorageService {storageServiceUrl}, Error: " + ex.Message);
            }
        }
    }
}
