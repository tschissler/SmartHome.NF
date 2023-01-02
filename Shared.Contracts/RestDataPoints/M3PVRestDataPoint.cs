using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.RestDataPoints
{
    public class PVM3RestDataPoint
    {
        public DateTime TimeStamp { get; set; }
        public double GridDemand { get; set; }
        public double GridSupply { get; set; }
        public double PVProduction { get; set; }
    }
}
