using PowerDogLib;

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
            var data = powerDog.ReadCloudDataCache();
            
        }
    }
}
