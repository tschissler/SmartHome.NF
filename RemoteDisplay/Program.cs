using NFLibs;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace RemoteDisplay
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("");
            Debug.WriteLine("#######################################");
            Debug.WriteLine($"# Remote Display version {Assembly.GetExecutingAssembly().GetName().Version}");
            Debug.WriteLine("#######################################");
            Debug.WriteLine("");

            Debug.Write("Connecting to Wifi ... ");
            var isWifiConnected = WifiLib.WifiLib.ConnectToWifi(Secrets.Ssid, Secrets.Password);
            if (!isWifiConnected)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Failed connecting to Wifi");
                return;
            }
            Debug.WriteLine("Done");

            Debug.Write("Initializing sensors ... ");
            //var isInitalized = I2CSensors.Init(32, 33, true, true, false, true);
            var isInitalized = I2CSensors.Init(21, 22, true, true, false, true);
            if (!isInitalized)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Failed initializing sensors");
                return;
            }
            Debug.WriteLine("Done");

            var display = new LEDMatrix(8);
            int cycle = 0;
            string text;
            int brightness = 1;

            while (true)
            {
                double light = I2CSensors.ReadBH1750Illuminance();
                brightness = (int)(light / 700 * 15);
                if (brightness > 15)
                    brightness = 15;

                Debug.WriteLine($"Illumination: {light} lux");
                Debug.WriteLine($"BMP180 Temperature: {I2CSensors.ReadBMP180Temperature().ToString("F1")}°C - BMP180 Pressure: {I2CSensors.ReadBMP180Pressure().ToString("F1")}hPa");
                Debug.WriteLine($"SHT31  Temperature: {I2CSensors.ReadSHT3xTemperature().ToString("F1")}°C - SHT31 Humidity: {I2CSensors.ReadSHT3xHumitidy().ToString("F0")}%");
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
                    //                    text = $"{I2CSensors.ReadSHTC3Temperature().ToString("F1")}°C/{I2CSensors.ReadSHTC3Humitidy().ToString("F0")}%";
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
    }
}
