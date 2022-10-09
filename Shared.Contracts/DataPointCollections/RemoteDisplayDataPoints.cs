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
        public DecimalDataPoint Temperature = new() { Label="Temperatur", Unit="°C", MaxValue=30, DecimalDigits = 1, History = new() { DataHistoryLength = 100, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 144, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Humidity = new() { Label = "Luftfeuchtigkeit", Unit = "%", MaxValue = 100, DecimalDigits = 1, History = new() { DataHistoryLength = 100, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 144, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Pressure = new() { Label = "Luftdruck", Unit = "hPa", MaxValue = 1100, DecimalDigits = 0, History = new() { DataHistoryLength = 100, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 144, AggregationType = AggregationType.Average } };
        public DecimalDataPoint Illumination = new() { Label = "Beleuchtungsstärke", Unit = "lx", MaxValue = 1000, DecimalDigits = 0, History = new() { DataHistoryLength = 100, AggregationTimeSpan = TimeSpan.FromMinutes(10), AggregationHistoryLength = 144, AggregationType = AggregationType.Average } };
    }
}
