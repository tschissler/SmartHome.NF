using SharedContracts.DataPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.DataPointCollections
{
    public class RemoteDisplayDataPoints
    {
        public DecimalDataPoint Temperature     = new() { Label = "Temperatur Esszimmer",      Unit = "°C",    MaxValue = 30,   DecimalDigits = 1, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(5), AggregationHistoryLength = 2880, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Humidity        = new() { Label = "Luftfeuchtigkeit",          Unit = "%",     MaxValue = 100,  DecimalDigits = 1, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(5), AggregationHistoryLength = 2880, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Pressure        = new() { Label = "Luftdruck",                 Unit = "hPa",   MaxValue = 1100, DecimalDigits = 0, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(5), AggregationHistoryLength = 2880, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Illumination    = new() { Label = "Beleuchtungsstärke",        Unit = "lx",    MaxValue = 1000, DecimalDigits = 0, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(5), AggregationHistoryLength = 2880, AggregationType = AggregationType.Average } };
        public DecimalDataPoint TempKidsRoom    = new() { Label = "Temp Ist KiZi",             Unit = "°C",    MaxValue = 30,   DecimalDigits = 1, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint TempBathRoom    = new() { Label = "Temp Ist Bad",              Unit = "°C",    MaxValue = 30,   DecimalDigits = 1, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint TempSetKidsRoom = new() { Label = "Temp Soll KiZi",            Unit = "°C",    MaxValue = 40,   DecimalDigits = 1, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint TempSetBathRoom = new() { Label = "Temp Soll Bad",             Unit = "°C",    MaxValue = 40,   DecimalDigits = 1, History = new() { DataHistoryLength = 144, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public BooleanDataPoint WindowKidsRoom  = new() { Label = "Fenster KiZi" };
        public BooleanDataPoint WindowBathRoom  = new() { Label = "Fenster Bad" };
    }
}
