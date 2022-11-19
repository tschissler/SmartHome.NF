using SharedContracts.DataPoints;

namespace SharedContracts.DataPointCollections
{
    public class PVDataPoints
    {
        public DecimalDataPoint PVProduction = new() { Label = "Produktion",  Unit = "W", MaxValue = 12000, DecimalDigits = 1, History = new() { DataHistoryLength = 300, AggregationTimeSpan = TimeSpan.FromMinutes(1), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average }, CurrentValueCorrection = (5.7 + 5.13) / 5.7 };
        public DecimalDataPoint GridSupply   = new() { Label = "Einspeisung", Unit = "W", MaxValue = 12000, DecimalDigits = 1, History = new() { DataHistoryLength = 300, AggregationTimeSpan = TimeSpan.FromMinutes(1), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
        public DecimalDataPoint GridDemand   = new() { Label = "Bezug",       Unit = "W", MaxValue = 8000,  DecimalDigits = 1, History = new() { DataHistoryLength = 300, AggregationTimeSpan = TimeSpan.FromMinutes(1), AggregationHistoryLength = 1440, AggregationType = AggregationType.Average } };
    }
}
