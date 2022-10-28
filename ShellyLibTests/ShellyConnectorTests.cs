using FluentAssertions;
using ShellyLib;
using System.Net;

namespace ShellyLibTests
{
    [TestClass]
    public class ShellyConnectorTests
    {
        [TestMethod]
        public void ReadPower()
        {
            var result = ShellyConnector.ReadPower(new IPAddress(new byte[] { 192, 168, 178, 177 }));
            result.Should().BeGreaterThan(0);
        }
    }
}