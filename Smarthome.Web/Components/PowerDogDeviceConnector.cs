using PowerDog;

namespace Smarthome.Web.Components
{
    public class DataChangedEventArgs : EventArgs
    {
        public DataChangedEventArgs(double value)
        {
            Value = value;
        }
        public double Value { get; }
    }

    public class PowerDogDeviceConnector
    {
        private Dictionary<string, string> sensorKeys = new()
            {
                { "Bezug", "iec1107_1457430339" }, // Vom Zähler
                { "Erzeugung", "pv_global_1454412642" },  // Vom Wechselrichter
                { "Eigenverbrauch", "arithmetic_1457431399" },
                { "Verbrauchgesamt", "arithmetic_1457432629" },
                { "lieferung", "iec1107_1457430562" } // Vom Zähler
            };
        
        private Timer refreshDataTimer;
        private Uri powerDogUri;
        private string powerDogPassword;
       
        public delegate void PVProductionChangedEventHandler(object sender, EventArgs e);
        public delegate void GridConsumptionChangedEventHandler(object sender, EventArgs e);
        public delegate void GridSupplyChangedEventHandler(object sender, EventArgs e);

        public event PVProductionChangedEventHandler PVProductionChanged;
        public event GridConsumptionChangedEventHandler GridConsumptionChanged;
        public event GridSupplyChangedEventHandler GridSupplyChanged;

        public void InitializePowerDogDeviceConnector(Uri powerDogUri, string powerDogPassword, TimeSpan readDeviceDataInterval)
        {
            this.powerDogUri = powerDogUri;
            this.powerDogPassword = powerDogPassword;
            refreshDataTimer = new Timer(new TimerCallback(RefreshData), null, 0, (int)readDeviceDataInterval.TotalMilliseconds);
        }

        private void RefreshData(object? state)
        {
            PowerDog.PowerDog target = new(sensorKeys, powerDogUri, powerDogPassword);
            var data = target.ReadSensorsData();

            if (data != null)
            {
                if (data["Erzeugung"] != null)
                {
                    PVProductionChanged?.Invoke(this, new DataChangedEventArgs((double)data["Erzeugung"]));
                }
                if (data["Bezug"] != null)
                {
                    GridConsumptionChanged?.Invoke(this, new DataChangedEventArgs((double)data["Bezug"]));
                }
                if (data["lieferung"] != null)
                {
                    GridSupplyChanged?.Invoke(this, new DataChangedEventArgs((double)data["lieferung"]));
                }
            }
        }
    }
}
