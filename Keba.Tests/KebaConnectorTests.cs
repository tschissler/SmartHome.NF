using FluentAssertions;
using KebaLib;
using System.Diagnostics;
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
            connector.Dispose();
            
            actual.Should().NotBeEmpty();
        }

        [TestMethod]
        public void TestReadingDeviceReport1()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceReport1();
            connector.Dispose();

            Console.WriteLine(actual);
            actual.Should().NotBeEmpty();
            actual.Should().Contain("\"Serial\": \"22588720\",");
        }

        [TestMethod]
        public void TestReadingDeviceReport2()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceReport2();
            connector.Dispose();

            Console.WriteLine(actual);
            actual.Should().NotBeEmpty();
            actual.Should().Contain("\"Serial\": \"22588720\",");
        }

        [TestMethod]
        public void TestReadingDeviceStatus()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            var actual = connector.GetDeviceStatus();
            actual.Serial.Should().NotBeEmpty();
            actual.EnergyTotal.Should().BeGreaterThan(0);
            actual = connector.GetDeviceStatus();
            actual.Serial.Should().NotBeEmpty();
            actual.EnergyTotal.Should().BeGreaterThan(0);
            actual = connector.GetDeviceStatus();
            actual.Serial.Should().NotBeEmpty();
            actual.EnergyTotal.Should().BeGreaterThan(0);
            connector.Dispose();
        }

        [TestMethod]
        public void TestSetCurrentDirectCommands()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);

            Console.WriteLine("-------> Executing report 2");
            Console.WriteLine(connector.ExecuteUDPCommand("report 2"));
            Console.WriteLine("-------> currtime 6000 10");
            Console.WriteLine(connector.ExecuteUDPCommand("currtime 6000 10"));
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(2000);
                Console.WriteLine("-------> Executing report 2");
                Console.WriteLine(connector.ExecuteUDPCommand("report 2"));
            }
            Console.WriteLine("-------> currtime 0 1");
            Console.WriteLine(connector.ExecuteUDPCommand("currtime 0 1"));

            for (int i = 0; i < 4; i++)
            {
                Thread.Sleep(2000);
                Console.WriteLine("-------> Executing report 2");
                Console.WriteLine(connector.ExecuteUDPCommand("report 2"));
            }
        }

        [TestMethod]
        public void TestSetCurrent()
        {
            bool changeDetected = false;
            KebaDeviceStatusData actual = null;
            
            Stopwatch stopwatch = new();
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);

            connector.SetChargingCurrent(0);
            stopwatch.Start();
            changeDetected = false;

            while (!changeDetected && stopwatch.ElapsedMilliseconds < 10000)
            {
                Thread.Sleep(1000);
                actual = connector.GetDeviceStatus();
                changeDetected = actual.MaxCurrency == 0;
            }
            actual.Serial.Should().NotBeEmpty();
            actual.MaxCurrency.Should().Be(0);
            
            connector.SetChargingCurrent(7000);
            stopwatch.Start();
            changeDetected = false;
            while (!changeDetected && stopwatch.ElapsedMilliseconds < 10000)
            {
                Thread.Sleep(1000);
                actual = connector.GetDeviceStatus();
                changeDetected = actual.MaxCurrency == 7000;
            }
            actual.Serial.Should().NotBeEmpty();
            actual.MaxCurrency.Should().Be(7000);
           
            connector.SetChargingCurrent(0);
            stopwatch.Start();
            changeDetected = false;
            
            while (!changeDetected && stopwatch.ElapsedMilliseconds < 10000)
            {
                Thread.Sleep(1000);
                actual = connector.GetDeviceStatus();
                changeDetected = actual.MaxCurrency == 0;
            }
            actual.Serial.Should().NotBeEmpty();
            actual.MaxCurrency.Should().Be(0);
            connector.Dispose();
        }

        [TestMethod]
        public void TestPerformance()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);
            Stopwatch watch = new();
            watch.Start();
            var actual1 = connector.GetDeviceStatus();
            var time1 = watch.ElapsedMilliseconds;
            watch.Restart();
            connector.WriteChargingCurrentToDevice(6);
            var time2 = watch.ElapsedMilliseconds;
            watch.Restart();
            var actual3 = connector.GetDeviceStatus();
            var time3 = watch.ElapsedMilliseconds;
            Console.WriteLine($"Time 1: {time1} | Time 2: {time2} | Time 3: {time3}");
            actual1.EnergyTotal.Should().BeGreaterThan(0);
            actual3.EnergyTotal.Should().BeGreaterThan(0);
            time1.Should().BeLessThan(500);
            time2.Should().BeLessThan(3000);
            time3.Should().BeLessThan(500);
            connector.Dispose();
        }
        
        [TestMethod]
        [Ignore]
        public void TestLocking()
        {
            KebaDeviceConnector connector = new(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);

            var actual1 = Task.Run(() => connector.GetDeviceStatus());
            var actual2 = Task.Run(() => connector.SetChargingCurrent(0));
            var actual3 = Task.Run(() => connector.GetDeviceStatus());
            actual1.Wait();
            actual2.Wait();
            actual3.Wait();
            actual1.Result.EnergyTotal.Should().BeGreaterThan(0);
            actual3.Result.EnergyTotal.Should().BeGreaterThan(0);
            connector.Dispose();

        }
    }
}