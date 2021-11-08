using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Iot.Device.Hcsr04;
using nanoFramework.Azure.Devices.Client;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using UnitsNet;
using WiFiAP;

namespace SmartHome.NF
{
    public class Program
    {
        private static GpioController GpioController;
        private static GpioPin IsAliveLED;
        private static GpioPin TransmitLED;
        private static Hcsr04 DistanceSensor;
        private static DeviceClient azureIoT;
        private static WebServer server = new WebServer();
        private static int connectedCount = 0;
        public static string DeviceID = "M1_Keller";

        // One minute unit
        const int sleepTimeMinutes = 60000;

        public static void Main()
        {
            GpioController = new GpioController();
            var startLED = GpioController.OpenPin(21, PinMode.Output);
            startLED.Write(PinValue.High);

            Debug.WriteLine("----- SmartHome.NF ------");
            Debug.WriteLine("Initializing...");
            Debug.Write("   - Wifi...");
            bool isConnected = ConnectToWifi();
            if (isConnected)
            {
                Debug.WriteLine("Done");

                Debug.Write("   - Azure IoT Hub...");
                azureIoT = new DeviceClient(Secrets.IotBrokerAddress, DeviceID, Secrets.SasKey, azureCert: new X509Certificate(SmartHome.NF.Resources.GetBytes(SmartHome.NF.Resources.BinaryResources.AzureRoot)));
                var isOpen = azureIoT.Open(); 
                Debug.WriteLine("Done");

                Debug.Write("   - GPIO...");

                IsAliveLED = GpioController.OpenPin(22, PinMode.Output);
                IsAliveLED.Write(PinValue.Low);
                TransmitLED = GpioController.OpenPin(23, PinMode.Output);
                TransmitLED.Write(PinValue.Low);
                startLED.Write(PinValue.Low);
                Timer blinkTimer = new Timer(IsAliveBlink, null, 1000, (int)new TimeSpan(0, 0, 3).TotalMilliseconds);

                DistanceSensor = new Hcsr04(14, 12);
                Timer distanceTimer = new Timer(MeassureDistance, null, 1000, (int)new TimeSpan(0,10,0).TotalMilliseconds);
                Debug.WriteLine("Done");
            }
            Thread.Sleep(Timeout.Infinite);
        }

        static bool ConnectToWifi()
        {
            // As we are using TLS, we need a valid date & time
            // We will wait maximum 1 minute to get connected and have a valid date
            var success = NetworkHelper.ConnectWifiDhcp(Secrets.Ssid, Secrets.Password, setDateTime: true, token: new CancellationTokenSource(sleepTimeMinutes).Token);
            if (!success)
            {
                Debug.WriteLine($"Can't connect to wifi: {NetworkHelper.ConnectionError.Error}");
                if (NetworkHelper.ConnectionError.Exception != null)
                {
                    Debug.WriteLine($"NetworkHelper.ConnectionError.Exception");
                }
            }

            return success;
        }

        private static void ConnectWiFi()
        {
            if (!Wireless80211.IsEnabled())
            {
                Wireless80211.Disable();
                if (WirelessAP.Setup() == false)
                {
                    // Reboot device to Activate Access Point on restart
                    Debug.WriteLine($"Setup Soft AP, Rebooting device");
                    Power.RebootDevice();
                }

                Debug.WriteLine($"Running Soft AP, waiting for client to connect");
                Debug.WriteLine($"Soft AP IP address :{WirelessAP.GetIP()}");

                // Link up Network event to show Stations connecting/disconnecting to Access point.
                //NetworkChange.NetworkAPStationChanged += NetworkChange_NetworkAPStationChanged;
                // Now that the normal Wifi is deactivated, that we have setup a static IP
                // We can start the Web server
                server.Start();
                Thread.Sleep(Timeout.Infinite);

            }
            else
            {
                Debug.WriteLine($"Running in normal mode, connecting to Access point");
                var conf = Wireless80211.GetConfiguration();
                bool success;
                // For devices like STM32, the password can't be read
                if (string.IsNullOrEmpty(conf.Password))
                {
                    // In this case, we will let the automatic connection happen
                    success = NetworkHelper.ReconnectWifi(setDateTime: true, token: new CancellationTokenSource(60000).Token);
                }
                else
                {
                    // If we have access to the password, we will force the reconnection
                    // This is mainly for ESP32 which will connect normaly like that.
                    success = NetworkHelper.ConnectWifiDhcp(conf.Ssid, conf.Password, token: new CancellationTokenSource(60000).Token);
                }
                Debug.WriteLine($"Connection is {success}");
                if (success)
                {
                    string IpAdr = Wireless80211.WaitIP();
                    Debug.WriteLine($"Connected as {IpAdr}");
                    // We can even wait for a DateTime now
                    success = NetworkHelper.WaitForValidIPAndDate(true, NetworkInterfaceType.Wireless80211, new CancellationTokenSource(60000).Token);
                    if (success)
                    {
                        Debug.WriteLine($"We have a valid date: {DateTime.UtcNow}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Something wrong happened, can't connect at all");
                }
            }
        }

        private static void MeassureDistance(object state)
        {
            if (DistanceSensor.TryGetDistance(out Length distance))
            {
                TransmitLED.Write(PinValue.High);
                Debug.WriteLine($"Distance: {distance.Centimeters} cm");
                azureIoT.SendMessage($"{{\"DeviceUTCTime\":\"{DateTime.UtcNow}\",\"deviceId\":\"{DeviceID}\",\"ZisterneLevel\":{distance.Centimeters}}}");
                TransmitLED.Write(PinValue.Low);
            }
            else
            {
                Debug.WriteLine("Error reading sensor");
            }
        }
        private static void IsAliveBlink(object state)
        {
            IsAliveLED.Toggle();
            Thread.Sleep(10);
            IsAliveLED.Toggle();
            Thread.Sleep(200);
            IsAliveLED.Toggle();
            Thread.Sleep(10);
            IsAliveLED.Toggle();
        }

        /// <summary>
        /// Event handler for Stations connecting or Disconnecting
        /// </summary>
        /// <param name="NetworkIndex">The index of Network Interface raising event</param>
        /// <param name="e">Event argument</param>
        private static void NetworkChange_NetworkAPStationChanged(int NetworkIndex, NetworkAPStationEventArgs e)
        {
            Debug.WriteLine($"NetworkAPStationChanged event Index:{NetworkIndex} Connected:{e.IsConnected} Station:{e.StationIndex} ");

            // if connected then get information on the connecting station 
            if (e.IsConnected)
            {
                WirelessAPConfiguration wapconf = WirelessAPConfiguration.GetAllWirelessAPConfigurations()[0];
                WirelessAPStation station = wapconf.GetConnectedStations(e.StationIndex);

                string macString = BitConverter.ToString(station.MacAddres);
                Debug.WriteLine($"Station mac {macString} Rssi:{station.Rssi} PhyMode:{station.PhyModes} ");

                connectedCount++;

                // Start web server when it connects otherwise the bind to network will fail as 
                // no connected network. Start web server when first station connects 
                if (connectedCount == 1)
                {
                    // Wait for Station to be fully connected before starting web server
                    // other you will get a Network error
                    Thread.Sleep(2000);
                    server.Start();
                }
            }
            else
            {
                // Station disconnected. When no more station connected then stop web server
                if (connectedCount > 0)
                {
                    connectedCount--;
                    if (connectedCount == 0)
                        server.Stop();
                }
            }

        }
    }
}
