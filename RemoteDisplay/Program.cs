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

            var display = new LEDMatrix(8);
            bool showSeparators = true;
            string text;

            while (true)
            {
                if (showSeparators)
                {
                    text = $"{(DateTime.UtcNow.Hour+1).ToString("D2")}:{DateTime.UtcNow.Minute.ToString("D2")}:{DateTime.UtcNow.Second.ToString("D2")}";
                }
                else
                {
                    text = $"{(DateTime.UtcNow.Hour+1).ToString("D2")} {DateTime.UtcNow.Minute.ToString("D2")} {DateTime.UtcNow.Second.ToString("D2")}";
                }
                display.ShowText(text, 1, characterSpace:1);
                Thread.Sleep(400);
                showSeparators = !showSeparators;
            }

        }
    }
}
