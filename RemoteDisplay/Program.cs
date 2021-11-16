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
            var isWifiConnected = WifiLib.ConnectToWifi(Secrets.Ssid, Secrets.Password);
            if (!isWifiConnected)
            {
                Debug.WriteLine("");
                Debug.WriteLine("Failed connecting to Wifi");
                return;
            }
            Debug.WriteLine("Done");

            Debug.Write("Initializing sensors ... ");
            var isInitalized = I2CSensors.Init(32, 33, false, true, true);
            //var isInitalized = I2CSensors.Init(21, 22, false, true, true);
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
                if (cycle < 6)
                {
                    // Show clock
                    if (cycle % 2 == 0)
                    {
                        text = $"{(DateTime.UtcNow.Hour + 1).ToString("D2")}:{DateTime.UtcNow.Minute.ToString("D2")}:{DateTime.UtcNow.Second.ToString("D2")}";
                    }
                    else
                    {
                        text = $"{(DateTime.UtcNow.Hour + 1).ToString("D2")} {DateTime.UtcNow.Minute.ToString("D2")} {DateTime.UtcNow.Second.ToString("D2")}";
                    }
                    display.ShowText(text, brightness, characterSpace: 1);
                }
                else if (cycle < 12)
                {
                    text = $"{I2CSensors.ReadSHTC3Temperature().ToString("F1")}°C/{I2CSensors.ReadSHTC3Humitidy().ToString("F0")}%";
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
    }
}
