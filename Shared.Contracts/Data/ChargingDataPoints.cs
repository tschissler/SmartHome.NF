using System.Runtime.Serialization;

namespace SharedContracts.Data
{
    public class ChargingDataPoints
    {
        public DecimalDataPoint CarCharingActiveSession = new() { Unit = "Wh", MaxValue = 80000 };
        public DecimalDataPoint CarCharingTotal = new() { Unit = "KWh", DecimalDigits = 1 };
        public DecimalDataPoint CarLatestChargingPower = new() { Unit = "W", MaxValue = 12000, DecimalDigits = 1 };
        public DecimalDataPoint CarChargingCurrentTarget = new() { Unit = "A", MaxValue = 16, DecimalDigits = 1 };
        public IntegerDataPoint CarChargingManualCurrency = new() { Unit = "mA", MaxValue = 16000 };
        public IntegerDataPoint KebaStatus = new();
    }
}
