using ChargingController;
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
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceInformation();
            actual.Should().NotBeEmpty();
        }

        [TestMethod]
        public void TestReadingDeviceReport()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceReport();
            actual.Should().NotBeEmpty();
        }

        [TestMethod]
        public void TestReadingDeviceStatus()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceStatus();
            actual.Serial.Should().NotBeEmpty();
            actual.EnergyTotal.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void TestSetCurrent()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.SetChargingCurrent(7000);
            actual.Serial.Should().NotBeEmpty();
            actual.TargetCurrency.Should().Be(7000);
            actual = connector.SetChargingCurrent(63000);
            actual.Serial.Should().NotBeEmpty();
            actual.TargetCurrency.Should().Be(63000);
        }
    }
}