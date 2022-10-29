using FluentAssertions;
using ShellyLib;
using System.Net;

namespace ShellyLibTests
{
    [TestClass]
    public class ShellyConnectorTests
    {
        [TestMethod]
        public void ReadPowerFromPlug()
        {
            var result = ShellyConnector.ReadPlugPower(new IPAddress(new byte[] { 192, 168, 178, 177 }));
            result.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void ReadPowerFrom3EM()
        {
            var result = ShellyConnector.Read3EMPower(new IPAddress(new byte[] { 192, 168, 178, 179 }));
            result[0].Should().BeGreaterThan(0);
            result[1].Should().BeGreaterThan(0);
            result[2].Should().BeGreaterThan(0);
        }
    }
}