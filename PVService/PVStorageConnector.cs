using HelpersLib;
using Newtonsoft.Json;
using PowerDogLib;
using SharedContracts.DataPointCollections;

namespace PVService
{
    public class PVStorageConnector
    {
        private PowerDog powerDog;
        
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
                    Http.PostAsJsonAsync($"https://smarthomestorageservice.azurewebsites.net/addpvm3data", data, new CancellationToken());
                }

                ConsoleHelpers.PrintInformation($"Successfully submitted data to the cloud");
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage("Failed to read data from PVService, Error: " + ex.Message);
            }
        }
    }
}
