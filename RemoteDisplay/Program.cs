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


        }
    }
}
