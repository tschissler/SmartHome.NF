using Smarthome.Web.Components;

namespace Smarthome.Web.Data
{
    public class DataPoints
    {
        public DecimalDataPoint PVProduction = new ();
        public DecimalDataPoint GridSupply = new ();
        public DecimalDataPoint GridDemand = new ();

        public decimal a;

        public void InitializeDataPoints(PowerDogDeviceConnector powerDog)
        {
            powerDog.PVProductionChanged += (sender, e) => PVProduction.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridSupplyChanged += (sender, e) => a = ((DataChangedEventArgs)e).Value;
            powerDog.GridSupplyChanged += (sender, e) => GridSupply.CurrentValue = ((DataChangedEventArgs)e).Value;
            powerDog.GridConsumptionChanged += (sender, e) => GridDemand.CurrentValue = ((DataChangedEventArgs)e).Value;
        }
    }
}
