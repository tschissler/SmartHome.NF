
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
                { "Bezug", "iec1107_1457430339" }, // Vom Zähler
                { "Erzeugung", "pv_global_1454412642" },  // Vom Wechselrichter
                { "Eigenverbrauch", "arithmetic_1457431399" },
                { "Verbrauchgesamt", "arithmetic_1457432629" },
                { "lieferung", "iec1107_1457430562" } // Vom Zähler
            };

            UriBuilder uri = new ("http", "192.168.178.150", 20000);

            PowerDog target = new(sensorKeys, uri.Uri, "admin");
            var actual = target.ReadSensorsData();
            actual.Count.Should().Be(sensorKeys.Count);
            actual["Verbrauchgesamt"].Should().NotBe(null).And.BeGreaterThan(0);
        }
    }
}