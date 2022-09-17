using Keba;
using Smarthome.Web.Components;

namespace Smarthome.Web.Data
{
    public class DataPoints
    {
        public DecimalDataPoint PVProduction = new() { Unit = "W", MaxValue = 12000 };
        public DecimalDataPoint GridSupply = new () { Unit = "W", MaxValue = 12000 };
        public DecimalDataPoint GridDemand = new () { Unit = "W", MaxValue = 8000 };
        public DecimalDataPoint CarCharingCurrentSession = new() { Unit = "Wh", MaxValue = 80000 };
        public DecimalDataPoint CarCharingTotal = new() { Unit = "KWh" };
        public DecimalDataPoint CarCurrentChargingPower = new() { Unit = "W", MaxValue = 12000 };

        public void InitializeDataPoints(PowerDogDeviceConnector powerDog, KebaDeviceConnector keba)
        {
            powerDog.PVProductionChanged += (sender, e) => PVProduction.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridSupplyChanged += (sender, e) => GridSupply.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridConsumptionChanged += (sender, e) => GridDemand.CurrentValue = ((DataChangedEventArgs)e).Value;
            keba.KebaDeviceStatusChanged += (sender, e) =>
            {
                CarCharingCurrentSession.CurrentValue = ((KebaDataChangedEventArgs)e).Data.EnergyCurrentChargingSession;
                CarCharingTotal.CurrentValue = ((KebaDataChangedEventArgs)e).Data.EnergyTotal;
                CarCurrentChargingPower.CurrentValue = ((KebaDataChangedEventArgs)e).Data.Power;
            };

            // As the second PV is not considered, this is corrected
            PVProduction.CurrentValueCorrection = (double value) => { return value * (5.7 + 4.57)/5.7; };
            // Energy is in 0.1Wh, so we need to divide by 10
            CarCharingCurrentSession.CurrentValueCorrection = (double value) => { return value / 10; };
            CarCharingTotal.CurrentValueCorrection = (double value) => { return value / 10000; };
            CarCurrentChargingPower.CurrentValueCorrection = (double value) => { return value / 1000; };
        }
    }
}
