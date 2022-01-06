using nanoFramework.Azure.Devices.Client;
using NFLibs;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using GC = nanoFramework.Runtime.Native.GC;

namespace RemoteDisplay
{
    public class Program
    {
        private static DeviceClient azureIoT;
        //private const string DeviceID = "DevBoardRemoteDisplay";
        private const string DeviceID = "M1_RemoteDisplay";
        private static int TransmitInterval = (int)new TimeSpan(0, 10, 0).TotalMilliseconds;
        private const bool IsBMP180SensorPresent = true;
        private const bool IsBH1750SensorPresent = true;
        private const bool IsSHTC3SensorPresent = false;
        private const bool IsSHT3xSensorPresent = true;

        public static void Main()
        {
            Debug.WriteLine("");
            Debug.WriteLine("#######################################");
            Debug.WriteLine($"# Remote Display version {Assembly.GetExecutingAssembly().GetName().Version}");
            Debug.WriteLine("#######################################");
            Debug.WriteLine("");

            Debug.Write("Initializing sensors ... ");
            //var isInitalized = I2CSensors.Init(33, 32, IsBMP180SensorPresent, IsBH1750SensorPresent, IsSHTC3SensorPresent, IsSHT3xSensorPresent);
            //var isInitalized = I2CSensors.Init(32, 33, true, true, false, true);
            var isInitalized = I2CSensors.Init(21, 22, IsBMP180SensorPresent, IsBH1750SensorPresent, IsSHTC3SensorPresent, IsSHT3xSensorPresent);
            if (!isInitalized)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Failed initializing sensors");
                return;
            }
            Debug.WriteLine("Done");

            Debug.Write("Connecting to Wifi ... ");
            var isWifiConnected = WifiLib.WifiLib.ConnectToWifi(Secrets.Ssid, Secrets.Password);
            if (!isWifiConnected)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Failed connecting to Wifi");
                return;
            }

            if (isWifiConnected)
            {
                Debug.WriteLine("Done");

                Debug.Write("Connecting to Azure IoT Hub...");
                azureIoT = new DeviceClient(Secrets.IotBrokerAddress, DeviceID, Secrets.SasKey, azureCert: new X509Certificate(RemoteDisplay.Resources.GetBytes(RemoteDisplay.Resources.BinaryResources.AzureRoot)));
                var isOpen = azureIoT.Open();
                if (isOpen)
                {
                    Debug.WriteLine("Done");
                    Timer transmitTimer = new Timer(TransmitDataToIotHub, null, 0, TransmitInterval);
                }
                else
                {
                    Debug.WriteLine("Failed Connecting to Azure IoT Hub");
                }
            }

            var display = new LEDMatrix(8);
            int cycle = 0;
            string text;
            int brightness = 1;

            while (true)
            {
                if (IsBH1750SensorPresent)
                {
                    double light = I2CSensors.ReadBH1750Illuminance();
                    brightness = (int)(light / 700 * 15);
                    if (brightness > 15)
                        brightness = 15;
                }
                else
                {
                    brightness = 5;
                }

                if (cycle == 1)
                {
                    if (IsBH1750SensorPresent)
                    {
                        Debug.WriteLine($"Illumination: {I2CSensors.ReadBH1750Illuminance()} lux");
                    }
                    if (IsBMP180SensorPresent)
                    {
                        Debug.WriteLine($"BMP180 Temperature: {I2CSensors.ReadBMP180Temperature().ToString("F1")}°C - BMP180 Pressure: {I2CSensors.ReadBMP180Pressure().ToString("F1")}hPa");
                    }
                    if (IsSHT3xSensorPresent)
                    {
                        Debug.WriteLine($"SHT31  Temperature: {I2CSensors.ReadSHT3xTemperature().ToString("F1")}°C - SHT31 Humidity: {I2CSensors.ReadSHT3xHumitidy().ToString("F0")}%");
                    }
                    if (IsSHTC3SensorPresent)
                    {
                        Debug.WriteLine($"SHTC3  Temperature: {I2CSensors.ReadSHTC3Temperature().ToString("F1")}°C - SHT31 Humidity: {I2CSensors.ReadSHTC3Humitidy().ToString("F0")}%");
                    }
                }

                if (cycle < 6)
                {
                    // Show clock
                    var hour = DateTime.UtcNow.Hour;
                    var minute = DateTime.UtcNow.Minute;
                    var second = DateTime.UtcNow.Second;
                    
                    if (IsSummerTime(DateTime.UtcNow.Day, DateTime.UtcNow.Month, DateTime.UtcNow.DayOfWeek))
                    {
                        hour += 2;
                    }
                    else
                    {
                        hour++;
                    }
                    if (hour >= 24)
                    {
                        hour-=24;
                    }

                    if (cycle % 2 == 0)
                    {
                        text = $"{hour.ToString("D2")}:{minute.ToString("D2")}:{second.ToString("D2")}";
                    }
                    else
                    {
                        text = $"{hour.ToString("D2")} {minute.ToString("D2")} {second.ToString("D2")}";
                    }
                    display.ShowText(text, brightness, characterSpace: 1);
                }
                else if (cycle < 12)
                {
                    if (IsSHTC3SensorPresent)
                        text = $"{I2CSensors.ReadSHTC3Temperature().ToString("F1")}°C/{I2CSensors.ReadSHTC3Humitidy().ToString("F0")}%";
                    if (IsBMP180SensorPresent)
                        text = $"{I2CSensors.ReadBMP180Temperature().ToString("F1")}°C";
                    display.ShowText(text, brightness, characterSpace: 1);
                }
                else
                {
                    cycle = 0;
                }

                Thread.Sleep(300);
                cycle++;
            }

        }

        public static bool IsSummerTime(int day, int month, DayOfWeek dayOfWeek)
        {
            if (month < 3 || month > 10) return false;
            if (month > 3 && month < 10) return true;

            int previousSunday = day - (int)dayOfWeek;

            if (month == 3) return previousSunday >= 25;
            if (month == 10) return previousSunday < 25;

            return false; //this line never gonna happend
        }

        private static void TransmitDataToIotHub(object state)
        {
            try
            {
                var message = new StringBuilder();
                message.Append($"{{\"DeviceUTCTime\":\"{DateTime.UtcNow}\",\"deviceId\":\"{DeviceID}\"");

                double light = I2CSensors.ReadBH1750Illuminance();

                if (IsBH1750SensorPresent)
                {
                    message.Append($",\"Illumination\":{I2CSensors.ReadBH1750Illuminance()}");
                }
                if (IsBMP180SensorPresent)
                {
                    message.Append($",\"Temp\":{I2CSensors.ReadBMP180Temperature()}");
                    message.Append($",\"Pressure\":{I2CSensors.ReadBMP180Pressure()}");
                }
                if (IsSHT3xSensorPresent)
                {
                    message.Append($",\"Temp2\":{I2CSensors.ReadSHT3xTemperature()}");
                    message.Append($",\"Humidity\":{I2CSensors.ReadSHT3xHumitidy()}");
                }
                if (IsSHTC3SensorPresent)
                {
                    message.Append($",\"Temp2\":{I2CSensors.ReadSHTC3Temperature()}");
                    message.Append($",\"Humidity\":{I2CSensors.ReadSHTC3Humitidy()}");
                }

                message.Append($",\"Memory\":{GC.Run(false)}");
                message.Append("}");

                var t = message.ToString();
                Debug.WriteLine($"Sending Message to Azure IoT Hub:\n {t}");
                if (azureIoT.SendMessage(message.ToString()))
                {
                    Debug.Write("Data has been transmitted to Azure IoT Hub");
                }
                else
                {
                    Debug.WriteLine("Error transmitting data to Azure IoT Hub");
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
