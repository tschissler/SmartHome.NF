using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Iot.Device.Hcsr04.Esp32;
using Iot.Device.Shtc3;
using nanoFramework.Azure.Devices.Client;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Json;
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
        private static ArrayList gasTriggerTimestamps1 = new();
        private static ArrayList gasTriggerTimestamps2 = new();
        private static ArrayList gasTriggerTimestamps = gasTriggerTimestamps1;
        private static GpioPin gasContact;
        private static ArrayList stromTriggerTimestamps1 = new();
        private static ArrayList stromTriggerTimestamps2 = new();
        private static ArrayList stromTriggerTimestamps = stromTriggerTimestamps1;
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
        private static int TransmitInterval = (int)new TimeSpan(0, 0, 15).TotalMilliseconds;
        private static int ReadDistanceInterval = (int)new TimeSpan(0, 0, 30).TotalMilliseconds;
        private static object lockObject = new object();

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

                Debug.Write("   - GPIO...");

                startLED.Write(PinValue.Low);

                DistanceSensor = new Hcsr04(DistanceSensor_Trigger_Pin, DistanceSensor_Echo_Pin);
                Configuration.SetPinFunction(I2CDataPin, DeviceFunction.I2C1_DATA);
                Configuration.SetPinFunction(I2CClockPin, DeviceFunction.I2C1_CLOCK);
                I2cConnectionSettings settings = new I2cConnectionSettings(1, Shtc3.DefaultI2cAddress);
                I2cDevice device = I2cDevice.Create(settings);
                TempHumiditySensor = new Shtc3(device);

                Timer readDistanceTimer = new Timer(ReadDistance, null, 0, ReadDistanceInterval);
                Timer transmitTimer = new Timer(TransmitDataToService, null, 0, TransmitInterval);

                //GpioController.OpenPin(19, PinMode.Input);

                gasContact = GpioController.OpenPin(gasPin, PinMode.InputPullDown);
                stromContact = GpioController.OpenPin(stromPin, PinMode.InputPullUp);

                //gasContact.SetDriveMode(PinMode.InputPullUp);

                // add a debounce timeout 
                gasContact.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 20);
                stromContact.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 20);
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

                    var status = LogManager.PingLogService(Secrets.KellerSecrets.PingUrl);
                    if (status == HttpStatusCode.OK)
                    {
                        pingLED = BlueLED;
                    }

                    pingLED.Write(PinValue.High);
                    Thread.Sleep(10);
                    pingLED.Toggle();
                    Thread.Sleep(1000);

                    Debug.WriteLine($"{gasContact.Read().ToString()} - {gasTriggerTimestamps.Count} | {stromContact.Read().ToString()} - {stromTriggerTimestamps.Count}");
                }
            }
            Thread.Sleep(Timeout.Infinite);
        }

        private static void TransmitDataToService(object state)
        {
            var data = new ConsumptionData();
            
            try
            {
                if (stromTriggerTimestamps == stromTriggerTimestamps1)
                {
                    stromTriggerTimestamps = stromTriggerTimestamps2;
                    data.PowerTriggerTimestamps = stromTriggerTimestamps1;
                }
                else
                {
                    stromTriggerTimestamps = stromTriggerTimestamps1;
                    data.PowerTriggerTimestamps = stromTriggerTimestamps2;
                }
                
                if (gasTriggerTimestamps == gasTriggerTimestamps1)
                {
                    gasTriggerTimestamps = gasTriggerTimestamps2;
                    data.GasTriggerTimestamps = gasTriggerTimestamps1;
                }
                else
                {
                    gasTriggerTimestamps = gasTriggerTimestamps1;
                    data.GasTriggerTimestamps = gasTriggerTimestamps2;
                }

                data.WaterLevel = distanceSum / distanceCount;
                distanceCount = 0;
                distanceSum = 0;

                if (TempHumiditySensor.TryGetTemperatureAndHumidity(out var temperature, out var relativeHumidity))
                {
                    data.Temperature = temperature.DegreesCelsius;
                    data.Humidity = relativeHumidity.Percent;
                    Debug.WriteLine($"Temp: {temperature.DegreesCelsius} °C");
                    Debug.WriteLine($"Humidity: {relativeHumidity.Percent} %");
                }
                else
                {
                    Debug.WriteLine("Error reading temperature and humidity");
                    IsAliveLED = RedLED;
                }

                var json = JsonConvert.SerializeObject(data);
                Debug.WriteLine($"Sending sensor data to SmartHome: {json}");

                data.GasTriggerTimestamps.Clear();
                data.PowerTriggerTimestamps.Clear();

                HttpClient httpClient = new();
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var responseMessage = httpClient.Post(Secrets.KellerSecrets.SensorDataServiceUrl, content);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Error posting sensor data: " + responseMessage.StatusCode + " - " + responseMessage.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error posting sensor data: " + ex.Message);
            }
        }

        private static void TriggerGasContact(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            gasTriggerTimestamps.Add(DateTime.UtcNow);
            if (gasTriggerTimestamps.Count > 1000)
            {
                gasTriggerTimestamps.RemoveAt(0);
            }
            Console.WriteLine("GasTrigger");
        }

        private static void TriggerStromContact(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            stromTriggerTimestamps.Add(DateTime.UtcNow);
            if (stromTriggerTimestamps.Count > 1000)
            {
                stromTriggerTimestamps.RemoveAt(0);
            }
            Console.WriteLine("StromTrigger");
        }

        private static void ReadDistance(object state)
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

                // Debug.WriteLine($"Available memory: {GC.Run(false)}");

                IsAliveLED.Write(PinValue.High);
                Thread.Sleep(10);
                IsAliveLED.Toggle();
                Thread.Sleep(200);
                IsAliveLED.Toggle();
                Thread.Sleep(10);
                IsAliveLED.Toggle();
            }
            catch (Exception ex)
            {
                IsAliveLED = RedLED;
                LogManager.SendLogMessage(logUrl, "Exception in ReadDistance: " + ex.Message);
            }
        }
    }

    public class ConsumptionData
    {
        public ArrayList GasTriggerTimestamps { get; set; }
        public ArrayList PowerTriggerTimestamps { get; set; }
        public double WaterLevel { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
