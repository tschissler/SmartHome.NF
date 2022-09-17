using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Iot.Device.Hcsr04.Esp32;
using Iot.Device.Shtc3;
using nanoFramework.Azure.Devices.Client;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using Secrets;
using SmartHome.NF.Logging;
using UnitsNet;
using WifiLib;
using GC = nanoFramework.Runtime.Native.GC;

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
        private static int connectedCount = 0;
        private static int gasCounter = 0;
        private static GpioPin gasContact;
        private static int stromCounter = 0;
        private static GpioPin stromContact;
        private static GpioPin RedLED;
        private static GpioPin GreenLED;
        private static GpioPin BlueLED;
        private static double minDistance = 999;
        private static double maxDistance;
        private static double distanceSum;
        private static int distanceCount;

        private static string logUrl = "https://smarthomeweb.azurewebsites.net/api/logging/";

#if DEV
        //Dev Board
        private const string DeviceID = "DevBoard";
        private const int Red_LED_Pin = 21;
        private const int Green_LED_Pin = 23;
        private const int Blue_LED_Pin = 22;
        private const int DistanceSensor_Trigger_Pin = 14;
        private const int DistanceSensor_Echo_Pin = 12;
        private const int I2CDataPin = 18;
        private const int I2CClockPin = 5;
        private const int gasPin = 19;
        private const int stromPin = 18;
        private static int TransmitInterval = (int)new TimeSpan(0, 10, 30).TotalMilliseconds;
        private static int BlinkInterval = (int)new TimeSpan(0, 0, 3).TotalMilliseconds;

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
        private const int gasPin = 19;
        private const int stromPin = 18;
        private static int TransmitInterval = (int)new TimeSpan(0, 10, 0).TotalMilliseconds;
        private static int BlinkInterval = (int)new TimeSpan(0, 0, 30).TotalMilliseconds;

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
            Debug.WriteLine($"Device: {DeviceID}");
            Debug.WriteLine("Initializing...");
            Debug.Write("   - Wifi...");
            bool isConnected = WifiLib.WifiLib.ConnectToWifi(KellerSecrets.Ssid, KellerSecrets.Password);
            if (isConnected)
            {
                Debug.WriteLine("Done");

                Debug.Write("   - Azure IoT Hub...");
                azureIoT = new DeviceClient(KellerSecrets.IotBrokerAddress, DeviceID, KellerSecrets.SasKey, azureCert: new X509Certificate(SmartHome.NF.Resources.GetBytes(SmartHome.NF.Resources.BinaryResources.AzureRoot)));
                var isOpen = azureIoT.Open();
                Debug.WriteLine("Done");

                Debug.Write("   - Logging Service...");
                LogManager.SendLogMessage(logUrl, "Application started, logging enabeled");
                Debug.WriteLine("Done");

                Debug.Write("   - GPIO...");

                startLED.Write(PinValue.Low);

                DistanceSensor = new Hcsr04(DistanceSensor_Trigger_Pin, DistanceSensor_Echo_Pin);
                Configuration.SetPinFunction(I2CDataPin, DeviceFunction.I2C1_DATA);
                Configuration.SetPinFunction(I2CClockPin, DeviceFunction.I2C1_CLOCK);
                I2cConnectionSettings settings = new I2cConnectionSettings(1, Shtc3.DefaultI2cAddress);
                I2cDevice device = I2cDevice.Create(settings);
                TempHumiditySensor = new Shtc3(device);

                Timer blinkTimer = new Timer(IsAliveBlink, null, 0, BlinkInterval);
                Timer transmitTimer = new Timer(TransmitDataToIotHub, null, 0, TransmitInterval);

                //GpioController.OpenPin(19, PinMode.Input);

                gasContact = GpioController.OpenPin(gasPin, PinMode.InputPullDown);
                stromContact = GpioController.OpenPin(stromPin, PinMode.InputPullUp);

                //gasContact.SetDriveMode(PinMode.InputPullUp);

                // add a debounce timeout 
                //gasContact.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 20);
                //gasContact.ValueChanged += TriggerGasContact;


                GpioController.RegisterCallbackForPinValueChangedEvent(
                    gasPin,
                    PinEventTypes.Falling,
                    TriggerGasContact);

                GpioController.RegisterCallbackForPinValueChangedEvent(
                    stromPin,
                    PinEventTypes.Falling,
                    TriggerStromContact);

                Debug.WriteLine("Done");


                while (true)
                {
                    var pingLED = RedLED;

                    var status = LogManager.PingLogService(logUrl);
                    if (status == HttpStatusCode.OK)
                    {
                        pingLED = BlueLED;
                    }

                    pingLED.Write(PinValue.High);
                    Thread.Sleep(10);
                    pingLED.Toggle();
                    Thread.Sleep(1000);
                }
            }
            Thread.Sleep(Timeout.Infinite);
        }

        private static void TriggerGasContact(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            gasCounter++;
        }

        private static void TriggerStromContact(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            stromCounter++;
        }

        private static void TransmitDataToIotHub(object state)
        {
            try
            {
                IsAliveLED = GreenLED;

                var message = new StringBuilder();
                message.Append($"{{\"DeviceUTCTime\":\"{DateTime.UtcNow}\",\"deviceId\":\"{DeviceID}\",\"GasPulse\":{gasCounter},\"StromPulse\":{stromCounter}");

                gasCounter = 0;
                stromCounter = 0;

                if (distanceCount > 0)
                {
                    message.Append($",\"ZisterneLevel\":{distanceSum / distanceCount}");

                    minDistance = 999;
                    maxDistance = 0;
                    distanceCount = 0;
                    distanceSum = 0;
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

                message.Append($",\"Memory\":{GC.Run(false)}");
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
                TransmitLED.Write(PinValue.Low);
            }
            catch (Exception ex)
            {
                IsAliveLED = RedLED;
                LogManager.SendLogMessage(logUrl, "Exception in TransmitDataToIotHub: " + ex.Message);
            }
        }
        private static void IsAliveBlink(object state)
        {
            try
            {
                if (DistanceSensor.TryGetDistance(out Length distance))
                {
                    distanceSum += distance.Centimeters;
                    distanceCount++;
                    Debug.WriteLine($"Distance: {distance.Centimeters} cm");
                    //LogManager.SendLogMessage(logUrl, $"Distance: {distance.Centimeters} cm");

                    if (distance.Centimeters < minDistance)
                        minDistance = distance.Centimeters;
                    if (distance.Centimeters > maxDistance)
                        maxDistance = distance.Centimeters;
                }
                else
                {
                    Debug.WriteLine("Error reading sensor for distance");
                    IsAliveLED = RedLED;
                }

                if (distanceCount > 0)
                {
                    Debug.WriteLine($"Min: {minDistance} | Max: {maxDistance} | Var: {maxDistance - minDistance} | Avg: {distanceSum / distanceCount}");
                }

                Debug.WriteLine($"Available memory: {GC.Run(false)}");

                IsAliveLED.Write(PinValue.High);
                Thread.Sleep(10);
                IsAliveLED.Toggle();
                Thread.Sleep(200);
                IsAliveLED.Toggle();
                Thread.Sleep(10);
                IsAliveLED.Toggle();

                Debug.WriteLine($"{gasContact.Read().ToString()} - {gasCounter} | {stromContact.Read().ToString()} - {stromCounter}");
            }
            catch (Exception ex)
            {
                IsAliveLED = RedLED;
                LogManager.SendLogMessage(logUrl, "Exception in IsAliveBlink: " + ex.Message);
            }
        }

    }
}
