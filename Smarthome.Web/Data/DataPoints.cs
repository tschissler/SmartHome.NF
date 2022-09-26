using Keba;
using Smarthome.Web.Components;
using Smarthome.Web.Controllers;

namespace Smarthome.Web.Data
{
    public class DataPoints
    {
        public DecimalDataPoint PVProduction = new() { Unit = "W", MaxValue = 12000, DecimalDigits=1 };
        public DecimalDataPoint GridSupply = new () { Unit = "W", MaxValue = 12000, DecimalDigits=1 };
        public DecimalDataPoint GridDemand = new () { Unit = "W", MaxValue = 8000, DecimalDigits=1 };
        public DecimalDataPoint CarCharingActiveSession = new() { Unit = "Wh", MaxValue = 80000 };
        public DecimalDataPoint CarCharingTotal = new() { Unit = "KWh", DecimalDigits=1 };
        public DecimalDataPoint CarLatestChargingPower = new() { Unit = "W", MaxValue = 12000, DecimalDigits=1 };
        public DecimalDataPoint CarChargingCurrentTarget = new() { Unit = "A", MaxValue = 16, DecimalDigits = 1 };
        public IntegerDataPoint CarChargingManualCurrency = new() { Unit = "mA", MaxValue = 16000 };
        public IntegerDataPoint KebaStatus = new();
        public BooleanDataPoint PVCharging = new();
        public BooleanDataPoint MinimumCharging = new();

        public DataPoints(PowerDogDeviceConnector powerDog, KebaDeviceConnector keba, ChargingController chargingController)
        {
            powerDog.PVProductionChanged += (sender, e) => PVProduction.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridSupplyChanged += (sender, e) => GridSupply.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridConsumptionChanged += (sender, e) => GridDemand.CurrentValue = ((DataChangedEventArgs)e).Value;
            keba.KebaDeviceStatusChanged += (sender, e) =>
            {
                CarCharingActiveSession.CurrentValue = ((KebaDataChangedEventArgs)e).Data.EnergyCurrentChargingSession;
                CarCharingTotal.CurrentValue = ((KebaDataChangedEventArgs)e).Data.EnergyTotal;
                CarLatestChargingPower.CurrentValue = ((KebaDataChangedEventArgs)e).Data.CurrentChargingPower;
                CarChargingCurrentTarget.CurrentValue = ((KebaDataChangedEventArgs)e).Data.TargetCurrency;
                KebaStatus.CurrentValue = ((KebaDataChangedEventArgs)e).Data.State;
            };

            PVCharging.CurrentValue = chargingController.AutoCharging;
            CarChargingManualCurrency.CurrentValue = chargingController.ManualChargingCurrency;

            // Energy is in 0.1Wh, so we need to divide by 10
            CarCharingActiveSession.CurrentValueCorrection = (double value) => { return value / 10; };
            CarCharingTotal.CurrentValueCorrection = (double value) => { return value / 10000; };
            CarLatestChargingPower.CurrentValueCorrection = (double value) => { return value / 1000; };
            CarChargingCurrentTarget.CurrentValueCorrection = (double value) => { return value / 1000; };
        }
    }
}
