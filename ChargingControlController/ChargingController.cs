using HelpersLib;
using KebaLib;
using Newtonsoft.Json;
using PowerDogLib;
using Secrets;
using SharedContracts.DataPointCollections;
using System.Net;
using static System.Net.WebRequestMethods;

namespace ChargingControlController
{
    public class ChargingController
    {
        private PowerDog powerDog;
        private double lastSetChargingCurrency;
        
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
            HttpClient Http = new HttpClient();
            ChargingDataPoints kebaDataPoints = new();
            
            powerDog.ReadSensorsData(state);
            DataPoints.GridSupply.SetCorrectedValue(powerDog.DataPoints.GridSupply.CurrentValue);
            DataPoints.GridDemand.SetCorrectedValue(powerDog.DataPoints.GridDemand.CurrentValue);
            // Calculation
            DataPoints.GridSaldo.CurrentValue = DataPoints.GridSupply.CurrentValue - DataPoints.GridDemand.CurrentValue;

            try
            {
                var jsonString = Http.GetStringAsync("http://localhost:5004/readdata").Result;
                kebaDataPoints = JsonConvert.DeserializeObject<ChargingDataPoints>(jsonString);
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage("Failed to read data from ChargingController, Error: " + ex.Message);
            }

            DataPoints.CurrentChargingPower.CurrentValue = kebaDataPoints.CarLatestChargingPower.CurrentValue;
            DataPoints.CurrentVoltage.CurrentValue = kebaDataPoints.CurrentVoltage.CurrentValue;
            
            // Calculation
            DataPoints.CalculatedChargingPower.CurrentValue = DataPoints.GridSaldo.CurrentValue + DataPoints.CurrentChargingPower.CurrentValue;
            DataPoints.CalculatedChargingCurrency.CurrentValue = DataPoints.CalculatedChargingPower.CurrentValue > 0 ? DataPoints.CalculatedChargingPower.CurrentValue / 230 / 3 : 0;
            DataPoints.MinimumActivationPVCurrency.CurrentValue = DataPoints.MinimumChargingCurrency.CurrentValue * DataPoints.MinimumPVShare.CurrentValue / 100;
            if (DataPoints.CalculatedChargingCurrency.CurrentValue >= DataPoints.MinimumChargingCurrency.CurrentValue)
            {
                DataPoints.EffectiveCharingCurrency.CurrentValue = DataPoints.CalculatedChargingCurrency.CurrentValue;
            }
            else if (!DataPoints.AutomaticCharging.CurrentValue && DataPoints.ManualChargingCurrency.CurrentValue >= DataPoints.MinimumChargingCurrency.CurrentValue)
            {
                DataPoints.EffectiveCharingCurrency.CurrentValue = DataPoints.ManualChargingCurrency.CurrentValue;
            }
            else if (DataPoints.AutomaticCharging.CurrentValue && DataPoints.CalculatedChargingCurrency.CurrentValue >= DataPoints.MinimumActivationPVCurrency.CurrentValue)
            {
                DataPoints.EffectiveCharingCurrency.CurrentValue = DataPoints.MinimumChargingCurrency.CurrentValue;
            }
            else
            {
                DataPoints.EffectiveCharingCurrency.CurrentValue = 0;
            }
        }

        public void SetChargingCurrency(object? state)
        {
            if (DataPoints.EffectiveCharingCurrency.CurrentValue != lastSetChargingCurrency)
            {
                lastSetChargingCurrency = DataPoints.EffectiveCharingCurrency.CurrentValue;
                HttpClient Http = new HttpClient();
                try
                {
                    ConsoleHelpers.PrintMessage($"Calling service, updating value");
                    //Http.PostAsJsonAsync<double>("http://localhost:5004/setchargingcurrency", DataPoints.EffectiveCharingCurrency.CurrentValue).Wait();
                }
                catch (Exception ex)
                {
                    ConsoleHelpers.PrintErrorMessage("Failed to set charging currency, Error: " + ex.Message);
                }
            }
        }
    }
}
