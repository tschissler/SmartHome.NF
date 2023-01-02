using FluentAssertions;

namespace PowerDog.Tests
{
    [TestClass]
    public class PowerDogReadSensorDataTests
    {
        [TestMethod]
        public void ReadSensorDataSuccessfully()
        {
            Dictionary<string, string> sensorKeys = new()
            {
                { "Bezug", "iec1107_1457430339" }, // Vom Z�hler
                { "Erzeugung", "pv_global_1454412642" },  // Vom Wechselrichter
                { "Eigenverbrauch", "arithmetic_1457431399" },
                { "Verbrauchgesamt", "arithmetic_1457432629" },
                { "lieferung", "iec1107_1457430562" } // Vom Z�hler
            };

            UriBuilder uri = new ("http", "powerdog", 20000);

            PowerDogLib.PowerDog target = new(sensorKeys, uri.Uri, "admin");
            target.ReadSensorsData(null);
            (target.LocalDataPoints.PVProduction.CurrentValue + target.LocalDataPoints.GridDemand.CurrentValue).Should().BeGreaterThan(0);
        }
    }
}