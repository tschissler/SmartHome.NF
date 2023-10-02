using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.Configuration
{
    public class Common
    {
        public static string SyncfusionLicenseKey { get; set; }

        static Common()
        {
            if (Environment.GetEnvironmentVariable("SyncfusionLicenseKey") is string syncfusionLicenseKey)
            {
                SyncfusionLicenseKey = syncfusionLicenseKey;
            }
            else
            {
                throw new Exception("EnvironmentVariable SyncfusionLicenseKey not set, access to UI might be limited.");
            }
        }
    }
}
