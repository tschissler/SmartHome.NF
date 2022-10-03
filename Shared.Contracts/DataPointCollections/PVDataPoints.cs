using SharedContracts.DataPoints;

namespace SharedContracts.DataPointCollections
{
    public class PVDataPoints
    {
        public DecimalDataPoint PVProduction = new() { Label = "Produktion",  Unit = "W", MaxValue = 12000, DecimalDigits = 1, History = new() { DataHistoryLength = 100 }, CurrentValueCorrection = (5.7 + 4.57) / 5.7 };
        public DecimalDataPoint GridSupply   = new() { Label = "Einspeisung", Unit = "W", MaxValue = 12000, DecimalDigits = 1, History = new() { DataHistoryLength = 100 } };
        public DecimalDataPoint GridDemand   = new() { Label = "Bezug",       Unit = "W", MaxValue = 8000,  DecimalDigits = 1, History = new() { DataHistoryLength = 100 } };
    }
}
