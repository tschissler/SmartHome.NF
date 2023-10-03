using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.Configuration
{
    public class Common
    {
        public static string SyncfusionLicenseKey { get; set; }
        public static string PVServiceUrl { get; set; }
        public static string ChargingServiceUrl { get; set; }

        static Common()
        {
            foreach (var field in typeof(Common).GetProperties())
            {
                if (Environment.GetEnvironmentVariable(field.Name) is string envVariableValue)
                {
                    field.SetValue(null, envVariableValue);
                }
                else
                {
                    throw new Exception($"---> EnvironmentVariable {field.Name} is not set, execution will stop as a mandatory configuration is missing.");
                }
            }
        }
    }
}
