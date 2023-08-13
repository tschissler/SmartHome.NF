using HelpersLib;
using Newtonsoft.Json;
using SharedContracts.DataPointCollections;
using System.Net;

namespace ShellyLib
{
    public class ShellyConnector
    {
        public static double ReadPlugPower(IPAddress deviceAddress)
        {
            double value = 0;
            try
            {
                using (HttpClient Http = new HttpClient())
                {
                    var jsonString = Http.GetStringAsync($"http://{deviceAddress}/meter/0").Result;
                    value = JsonConvert.DeserializeObject<ShellyPlugMeterData>(jsonString).power;

                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Failed to read data from Shelly device {deviceAddress}, Error: " + ex.Message);
            }
            return value;
        }

        public static bool TurnRelayOn(IPAddress deviceAddress)
        {
            return TurnRelay(deviceAddress, true);
        }

        public static bool TurnRelayOff(IPAddress deviceAddress)
        {
            return TurnRelay(deviceAddress, false);
        }

        private static bool TurnRelay(IPAddress deviceAddress, bool turnOn)
        {
            bool value = false;
            try
            {
                var turnString = turnOn ? "on" : "off";

                using (HttpClient Http = new HttpClient())
                {
                    var jsonString = Http.GetStringAsync($"http://{deviceAddress}/relay/0?turn={turnString}").Result;
                    value = JsonConvert.DeserializeObject<TurnOnOffResponse>(jsonString).ison;
                }
                if (value != turnOn)
                {
                    throw new Exception($"Turning relay {turnString} did not succeed.");
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Failed to turn on relay on Shelly device {deviceAddress}, Error: " + ex.Message);
            }
            return value;
        }

        public static double[] Read3EMPower(IPAddress deviceAddress)
        {
            double[] value = new double[3];
            try
            {
                using (HttpClient Http = new HttpClient())
                {
                    var jsonString = Http.GetStringAsync($"http://{deviceAddress}/status").Result;
                    var data = JsonConvert.DeserializeObject<EM3Data>(jsonString);
                    value[0] = data.emeters[0].power;
                    value[1] = data.emeters[1].power;
                    value[2] = data.emeters[2].power;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Failed to read data from 3EM Shelly device {deviceAddress}, Error: " + ex.Message);
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


    public class EM3Data
    {
        public string time { get; set; }
        public int unixtime { get; set; }
        public int serial { get; set; }
        public bool has_update { get; set; }
        public string mac { get; set; }
        public int cfg_changed_cnt { get; set; }
        public Emeter[] emeters { get; set; }
        public double total_power { get; set; }
    }

    public class Emeter
    {
        public float power { get; set; }
        public float pf { get; set; }
        public float current { get; set; }
        public float voltage { get; set; }
        public bool is_valid { get; set; }
        public float total { get; set; }
        public float total_returned { get; set; }
    }


    public class TurnOnOffResponse
    {
        public bool ison { get; set; }
        public bool has_timer { get; set; }
        public int timer_started { get; set; }
        public int timer_duration { get; set; }
        public int timer_remaining { get; set; }
        public bool overpower { get; set; }
        public string source { get; set; }
    }

}