using HelpersLib;
using KebaLib;
using Newtonsoft.Json;
using PowerDogLib;
using Secrets;
using SharedContracts.DataPointCollections;
using System.Net;

namespace ChargingControlController
{
    public class ChargingController
    {
        private PowerDog powerDog;
        private KebaDeviceConnector kebaConnector;
        public ChargingControlDataPoints DataPoints;

        public ChargingController()
        {
            Dictionary<string, string> sensorKeys = new()
            {
                { "Bezug", "iec1107_1457430339" }, // Vom Zähler
                { "Erzeugung", "pv_global_1454412642" },  // Vom Wechselrichter
                { "Eigenverbrauch", "arithmetic_1457431399" },
                { "Verbrauchgesamt", "arithmetic_1457432629" },
                { "lieferung", "iec1107_1457430562" } // Vom Zähler
            };
            powerDog = new PowerDog(sensorKeys, new UriBuilder("http", "192.168.178.150", 20000).Uri, PowerDogSecrets.Password);

            DataPoints = new ChargingControlDataPoints();
        }

        public void CalculateData(object? state)
        {
            ChargingDataPoints kebaDataPoints = new();
            
            powerDog.ReadSensorsData(state);
            DataPoints.GridSupply.SetCorrectedValue(powerDog.DataPoints.GridSupply.CurrentValue);
            DataPoints.GridDemand.SetCorrectedValue(powerDog.DataPoints.GridDemand.CurrentValue);
            // Calculation
            DataPoints.GridSaldo.CurrentValue = DataPoints.GridSupply.CurrentValue - DataPoints.GridDemand.CurrentValue;

            try
            {
                HttpClient Http = new HttpClient();
                var jsonString = Http.GetStringAsync("http://localhost:5004/readdata").Result;
                kebaDataPoints = JsonConvert.DeserializeObject<ChargingDataPoints>(jsonString);
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage("Failed to read data from service, Error: " + ex.Message);
            }

            DataPoints.CurrentChargingPower.CurrentValue = kebaDataPoints.CarLatestChargingPower.CurrentValue;
            DataPoints.CurrentVoltage.CurrentValue = kebaDataPoints.CurrentVoltage.CurrentValue;
            
            // Calculation
            DataPoints.CalculatedChargingPower.CurrentValue = DataPoints.GridSaldo.CurrentValue + DataPoints.CurrentChargingPower.CurrentValue;
            DataPoints.CalculatedChargingCurrency.CurrentValue = DataPoints.CalculatedChargingPower.CurrentValue > 0 ? DataPoints.CalculatedChargingPower.CurrentValue / 230 : 0;
        }
    }
}
