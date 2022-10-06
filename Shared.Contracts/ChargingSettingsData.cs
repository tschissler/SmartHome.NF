using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts
{
    public class ChargingSettingsData
    {
        public bool automaticCharging { get; set; }
        public double minimumPVShare { get; set; }
        public double manualCurrency { get; set; }
        public bool isInitialized { get; set; }
    }
}
