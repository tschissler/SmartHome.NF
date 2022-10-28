using HelpersLib;
using Newtonsoft.Json;
using SharedContracts.DataPointCollections;
using System.Net;

namespace ShellyLib
{
    public class ShellyConnector
    {
        public static double ReadPower(IPAddress deviceAddress)
        {
            double value = 0;
            try
            {
                using (HttpClient Http = new HttpClient())
                {
                    var jsonString = Http.GetStringAsync($"http://{ deviceAddress}/meter/0").Result;
                    value = JsonConvert.DeserializeObject<ShellyPlugMeterData>(jsonString).power;

                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Failed to read data from Shelly device { deviceAddress}, Error: " + ex.Message);
            }
            return value;
        }
    }

    public class ShellyPlugMeterData
    {
        public double power { get; set; }
        public bool is_valid { get; set; }
        public double overpower { get; set; }
        public double timestamp { get; set; }
        public List<double> counters { get; set; }
        public double number { get; set; }
    }
}