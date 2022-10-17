using SharedContracts.DataPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.DataPointCollections
{
    public class ConsumptionDataPoints
    {
        public DecimalDataPoint Power = new() { Label = "Strom", Unit = "Wh", MaxValue = 5000, DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Gas   = new() { Label = "Gas",   Unit = "m³/h", MaxValue = 10, DecimalDigits = 2, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
    }
}
