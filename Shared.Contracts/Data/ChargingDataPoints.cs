using System.Runtime.Serialization;

namespace SharedContracts.Data
{
    public class ChargingDataPoints
    {
        public DecimalDataPoint CarCharingActiveSession = new() { Label="Ladung Sitzung", Unit = "Wh", MaxValue = 80000 };
        public DecimalDataPoint CarCharingTotal = new() { Label="Ladung gesamt", Unit = "KWh", DecimalDigits = 1 };
        public DecimalDataPoint CarLatestChargingPower = new() { Label="Ladeleistung", Unit = "W", MaxValue = 12000, DecimalDigits = 1 };
        public DecimalDataPoint CarChargingCurrentTarget = new() { Label="Strom Verfügbar", Unit = "A", MaxValue = 16, DecimalDigits = 1 };
        public IntegerDataPoint CarChargingManualCurrency = new() { Unit = "mA", MaxValue = 16000 };
        public IntegerDataPoint KebaStatus = new();
    }
}
