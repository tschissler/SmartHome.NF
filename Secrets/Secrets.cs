using Newtonsoft.Json;

namespace Secrets
{
    public class AzureSecrets
    {
        public static string AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=iotdatastorage;AccountKey=;EndpointSuffix=core.windows.net";
    }
    
    public class DeviceSecretsData
    {
        public string Ssid { get; set; }
        public string Password { get; set; }
        public string IotBrokerAddress { get; set; }
        public string SasKey { get; set; }
    }

    public class KellerSecrets
    {
        public static string Ssid { get { return _data.Ssid; } }
        public static string Password { get { return _data.Password; } }
        public static string IotBrokerAddress { get { return _data.IotBrokerAddress; } }
        public static string SasKey { get { return _data.SasKey; } }

        private static DeviceSecretsData _data;

        static KellerSecrets()
        {
            var jsonString = File.ReadAllText("Keller.Secrets.json");
            _data = JsonConvert.DeserializeObject<DeviceSecretsData>(jsonString);
        }
    }
    
    public class RemoteDisplaySecrets
    {
        public static string Ssid { get { return _data.Ssid; } }
        public static string Password { get { return _data.Password; } }
        public static string IotBrokerAddress { get { return _data.IotBrokerAddress; } }
        public static string SasKey { get { return _data.SasKey; } }

        private static DeviceSecretsData _data;
        
        static RemoteDisplaySecrets()
        {
            var jsonString = File.ReadAllText("RemoteDisplay.Secrets.json");
            _data = JsonConvert.DeserializeObject<DeviceSecretsData>(jsonString);
        }
    }

    public class PowerDogSecretsData
    {
        public string Password { get; set; }
    }
    public class PowerDogSecrets
    {
        public static string Password
        {
            get { return "admin"; }
        }
        //private static PowerDogSecretsData _data;

        //static PowerDogSecrets()
        //{
        //    var jsonString = File.ReadAllText("PowerDog.Secrets.json");
        //    _data = JsonConvert.DeserializeObject<PowerDogSecretsData>(jsonString);
        //}
    }
}