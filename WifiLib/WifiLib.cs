using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Threading;

namespace WifiLib
{
    public class WifiLib
    {
        // One minute 
        const int sleepTimeMinutes = 60000;

        public static bool ConnectToWifi(string ssid, string password)
        {
            // As we are using TLS, we need a valid date & time
            // We will wait maximum 1 minute to get connected and have a valid date
            var success = WifiNetworkHelper.ConnectDhcp(ssid, password, requiresDateTime: true, token: new CancellationTokenSource(sleepTimeMinutes).Token);
            if (!success)
            {
                Debug.WriteLine($"Can't connect to wifi: {WifiNetworkHelper.HelperException.Message}");
                if (WifiNetworkHelper.HelperException.InnerException != null)
                {
                    Debug.WriteLine($"Inner Exception: {WifiNetworkHelper.HelperException.InnerException.Message}");
                }
            }

            return success;
        }
    }
}
