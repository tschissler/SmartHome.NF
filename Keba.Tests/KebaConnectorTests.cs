using FluentAssertions;
using System.Net;

namespace Keba.Tests
{
    [TestClass]
    public class KebaConnectorTests
    {
        [TestMethod]
        public void TestReadingDeviceInformation()
        {
            KebaConnector connector = new (new IPAddress(new byte[] {192, 168, 178, 167}), 7090);
            var actual = connector.GetDeviceInformation();
            actual.Should().NotBeEmpty();
        }

        [TestMethod]
        public void TestReadingDeviceReport()
        {
            KebaConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceReport();
            actual.Should().NotBeEmpty();
        }

        [TestMethod]
        public void TestReadingDeviceStatus()
        {
            KebaConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceStatus();
            actual.Serial.Should().NotBeEmpty();
            actual.DefinedCurrency.Should().Be(63000);
        }
    }
}