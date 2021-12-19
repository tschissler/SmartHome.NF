using System;
using System.Collections;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
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
using UnitsNet;
using WiFiAP;
using WifiLib;

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
        private static ArrayList DistanceMeasures = new ArrayList();
        private static double minDistance = 999;
        private static double maxDistance;
        private static double sum;

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
        private const int ReedPin = 19;
        private static int TransmitInterval = (int)new TimeSpan(0, 0, 30).TotalMilliseconds;

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
            bool isConnected = WifiLib.WifiLib.ConnectToWifi(Secrets.Ssid, Secrets.Password);
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
                reedContact.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 300);
                reedContact.ValueChanged += TriggerReedContact;


                //GpioController.RegisterCallbackForPinValueChangedEvent(
                //    ReedPin,
                //    PinEventTypes.Falling | PinEventTypes.Rising,
                //    TriggerReedContact);

                Debug.WriteLine("Done");
            }
            Thread.Sleep(Timeout.Infinite);
        }

        private static void TriggerReedContact(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            reedCounter++;
        }

        private static void TransmitDataToIotHub(object state)
        {
            IsAliveLED = GreenLED;

            var message = new StringBuilder();
            message.Append($"{{\"DeviceUTCTime\":\"{DateTime.UtcNow}\",\"deviceId\":\"{DeviceID}\",\"GasPulse\":{reedCounter}");

            if (DistanceMeasures.Count > 0)
            {
                double distanceAverage = 0;
                foreach (Length distance in DistanceMeasures)
                {
                    distanceAverage += distance.Centimeters;
                }
                distanceAverage = distanceAverage / DistanceMeasures.Count;
                minDistance = 999;
                maxDistance = 0;
                DistanceMeasures = new ArrayList();

                message.Append($",\"ZisterneLevel\":{distanceAverage}");
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
            if (DistanceSensor.TryGetDistance(out Length distance))
            {
                DistanceMeasures.Add(distance);
                Debug.WriteLine($"Distance: {distance.Centimeters} cm");

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

            sum = 0;
            foreach (Length measurement in DistanceMeasures)
            {
                sum += measurement.Centimeters;
            }

            if (DistanceMeasures.Count > 0)
            {
                Debug.WriteLine($"Min: {minDistance} | Max: {maxDistance} | Var: {maxDistance - minDistance} | Avg: {sum / DistanceMeasures.Count}");
            }

            IsAliveLED.Toggle();
            Thread.Sleep(10);
            IsAliveLED.Toggle();
            Thread.Sleep(200);
            IsAliveLED.Toggle();
            Thread.Sleep(10);
            IsAliveLED.Toggle();

            Debug.WriteLine($"{reedContact.Read().ToString()} - {reedCounter}");
        }

    }
}
