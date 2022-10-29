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
        public DecimalDataPoint Power        = new() { Label = "Strom",             Unit = "W",    MaxValue = 5000, DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Gas          = new() { Label = "Gas",               Unit = "m³/h", MaxValue = 10,   DecimalDigits = 2, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint WaterLevel   = new() { Label = "Zisterne",          Unit = "cm",   MaxValue = 100,  DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Temperature  = new() { Label = "Temperatur Keller", Unit = "°C",   MaxValue = 25,   DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Humidity     = new() { Label = "Luftfeuchtigkeit",  Unit = "%",    MaxValue = 100,  DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint PowerDevice1 = new() { Label = "Strom Gerät 1",     Unit = "W",    MaxValue = 2000, DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint PowerDevice2 = new() { Label = "Strom Gerät 2",     Unit = "W",    MaxValue = 2000, DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint PowerPhase1  = new() { Label = "Strom L1",          Unit = "W",    MaxValue = 6000, DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint PowerPhase2  = new() { Label = "Strom L2",          Unit = "W",    MaxValue = 6000, DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint PowerPhase3  = new() { Label = "Strom L3",          Unit = "W",    MaxValue = 6000, DecimalDigits = 1, History = new() { DataHistoryLength = 200, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
    }
}
