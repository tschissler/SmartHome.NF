using Smarthome.Web.Components;

namespace Smarthome.Web.Data
{
    public class DataPoints
    {
        public DecimalDataPoint PVProduction = new() { Unit = "W", MaxValue = 12000 };
        public DecimalDataPoint GridSupply = new () { Unit = "W", MaxValue = 12000 };
        public DecimalDataPoint GridDemand = new () { Unit = "W", MaxValue = 8000 };

        public void InitializeDataPoints(PowerDogDeviceConnector powerDog)
        {
            powerDog.PVProductionChanged += (sender, e) => PVProduction.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridSupplyChanged += (sender, e) => GridSupply.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridConsumptionChanged += (sender, e) => GridDemand.CurrentValue = ((DataChangedEventArgs)e).Value;

            //GridDemand.CurrentValueCorrection = (double value) => { return value*2; }; 
        }
    }
}
