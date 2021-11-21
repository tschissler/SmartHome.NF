using System;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Iot.Device.Hcsr04;
using Iot.Device.Shtc3;
using nanoFramework.Azure.Devices.Client;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using NFLibs;
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
        private static Shtc3 TempHumiditySensor; 
        private static DeviceClient azureIoT;
        private static WebServer server = new WebServer();
        private static int connectedCount = 0;
        private static int reedCounter = 0;
        private static GpioPin reedContact;
        private static GpioPin RedLED;
        private static GpioPin GreenLED;
        private static GpioPin BlueLED;

#if DEBUG
        //Dev Board
        private const string DeviceID = "DevBoard";
        private const int Red_LED_Pin = 21;
        private const int Green_LED_Pin = 23;
        private const int Blue_LED_Pin = 22;
        private const int DistanceSensor_Trigger_Pin = 14;
        private const int DistanceSensor_Echo_Pin = 12;
        private const int I2CDataPin = 18;
        private const int I2CClockPin = 5;
        private const int ReedPin = 19;
        private static int TransmitInterval = (int)new TimeSpan(0, 0, 10).TotalMilliseconds;

#else

        // Prod Board
        private const string DeviceID = "M1_Keller";
        private const int Red_LED_Pin = 13;
        private const int Green_LED_Pin = 12;
        private const int Blue_LED_Pin = 27;
        private const int DistanceSensor_Trigger_Pin = 15;
        private const int DistanceSensor_Echo_Pin = 2;
        private const int I2CDataPin = 32;
        private const int I2CClockPin = 33;
        private const int ReedPin = 19;
        private static int TransmitInterval = (int)new TimeSpan(0, 10, 0).TotalMilliseconds;
#endif

        public static void Main()
        {
            GpioController = new GpioController();

            RedLED = GpioController.OpenPin(Red_LED_Pin, PinMode.Output);
            GreenLED = GpioController.OpenPin(Green_LED_Pin, PinMode.Output);
            BlueLED = GpioController.OpenPin(Blue_LED_Pin, PinMode.Output);
            RedLED.Write(PinValue.Low);
            GreenLED.Write(PinValue.Low);
            BlueLED.Write(PinValue.Low);

            var startLED = RedLED;
            startLED.Write(PinValue.High);
            IsAliveLED = GreenLED;
            TransmitLED = BlueLED;           

            Debug.WriteLine("----- SmartHome.NF ------");
            Debug.WriteLine("Initializing...");
            Debug.Write("   - Wifi...");
            bool isConnected = WifiLib.ConnectToWifi(Secrets.Ssid, Secrets.Password);
            if (isConnected)
            {
                Debug.WriteLine("Done");

                Debug.Write("   - Azure IoT Hub...");
                azureIoT = new DeviceClient(Secrets.IotBrokerAddress, DeviceID, Secrets.SasKey, azureCert: new X509Certificate(SmartHome.NF.Resources.GetBytes(SmartHome.NF.Resources.BinaryResources.AzureRoot)));
                var isOpen = azureIoT.Open(); 
                Debug.WriteLine("Done");

                Debug.Write("   - GPIO...");

                startLED.Write(PinValue.Low);
                Timer blinkTimer = new Timer(IsAliveBlink, null, 1000, (int)new TimeSpan(0, 0, 3).TotalMilliseconds);

                DistanceSensor = new Hcsr04(DistanceSensor_Trigger_Pin, DistanceSensor_Echo_Pin);
                Configuration.SetPinFunction(I2CDataPin, DeviceFunction.I2C1_DATA);
                Configuration.SetPinFunction(I2CClockPin, DeviceFunction.I2C1_CLOCK);
                I2cConnectionSettings settings = new I2cConnectionSettings(1, Shtc3.DefaultI2cAddress);
                I2cDevice device = I2cDevice.Create(settings);
                TempHumiditySensor = new Shtc3(device);

                Timer transmitTimer = new Timer(TransmitDataToIotHub, null, 1000, TransmitInterval);

                //GpioController.OpenPin(19, PinMode.Input);

                reedContact = GpioController.OpenPin(ReedPin, PinMode.InputPullUp);
                //reedContact.SetDriveMode(PinMode.InputPullUp);

                // add a debounce timeout 
                //reedContact.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 100);
                //reedContact.ValueChanged += TriggerReedContact;


                GpioController.RegisterCallbackForPinValueChangedEvent(
                    19,
                    PinEventTypes.Falling | PinEventTypes.Rising,
                    TriggerReedContact);

                Debug.WriteLine("Done");
            }
            Thread.Sleep(Timeout.Infinite);
        }

        private static void TriggerReedContact(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            reedCounter++;
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

        private static void TransmitDataToIotHub(object state)
        {
            IsAliveLED = GreenLED;

            var message = new StringBuilder();
            message.Append($"{{\"DeviceUTCTime\":\"{DateTime.UtcNow}\",\"deviceId\":\"{DeviceID}\",\"GasPulse\":{reedCounter}");

            if (DistanceSensor.TryGetDistance(out Length distance))
            {
                message.Append($",\"ZisterneLevel\":{distance.Centimeters}");
                Debug.WriteLine($"Distance: {distance.Centimeters} cm");
            }
            else
            {
                Debug.WriteLine("Error reading sensor for distance");
                IsAliveLED = RedLED;
            }

            if (TempHumiditySensor.TryGetTemperatureAndHumidity(out var temperature, out var relativeHumidity))
            {
                message.Append($",\"KellerTemp\":{temperature.DegreesCelsius},\"KellerHumidity\":{relativeHumidity.Percent}");
                Debug.WriteLine($"Temp: {temperature.DegreesCelsius} °C");
                Debug.WriteLine($"Humidity: {relativeHumidity.Percent} %");
            }
            else
            {
                Debug.WriteLine("Error reading temperature and humidity");
                IsAliveLED = RedLED;
            }

            message.Append("}");

            TransmitLED.Write(PinValue.High);
            var t = message.ToString();
            Debug.WriteLine(t);
            if (azureIoT.SendMessage(message.ToString()))
            {
                Debug.Write("Data has been transmitted to Azure IoT Hub");
            }
            else
            {
                Debug.WriteLine("Error transmitting data to Azure IoT Hub");
            }
            reedCounter = 0;
            TransmitLED.Write(PinValue.Low);

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

            Debug.WriteLine($"{reedContact.Read().ToString()} - {reedCounter}");
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
