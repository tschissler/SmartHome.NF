using Secrets;
using SharedContracts.DataPointCollections;

namespace ChargingControlController
{
    public class ChargingController
    {
        private PowerDog.PowerDog powerDog;
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
            powerDog = new PowerDog.PowerDog(sensorKeys, new UriBuilder("http", "192.168.178.150", 20000).Uri, PowerDogSecrets.Password);
            DataPoints = new ChargingControlDataPoints();

        }

        public void CalculateData(object? state)
        {
            powerDog.ReadSensorsData(state);
            DataPoints.GridSupply.SetCorrectedValue(powerDog.DataPoints.GridSupply.CurrentValue);
            DataPoints.GridDemand.SetCorrectedValue(powerDog.DataPoints.GridDemand.CurrentValue);
            DataPoints.GridSaldo.CurrentValue = DataPoints.GridSupply.CurrentValue - DataPoints.GridDemand.CurrentValue;
        }
    }
}
