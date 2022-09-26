using HelpersLib;
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
        private double pVProduction;

        // As the second PV is not considered, this is corrected
        public double PVProduction { get => pVProduction * (5.7 + 4.57) / 5.7; internal set => pVProduction = value; }
        public double GridSupply { get; internal set; }
        public double GridConsumption { get; internal set; }

        public delegate void PVProductionChangedEventHandler(object sender, EventArgs e);
        public delegate void GridConsumptionChangedEventHandler(object sender, EventArgs e);
        public delegate void GridSupplyChangedEventHandler(object sender, EventArgs e);

        public event PVProductionChangedEventHandler PVProductionChanged;
        public event GridConsumptionChangedEventHandler GridConsumptionChanged;
        public event GridSupplyChangedEventHandler GridSupplyChanged;

        public PowerDogDeviceConnector(Uri powerDogUri, string powerDogPassword, TimeSpan readDeviceDataInterval)
        {
            this.powerDogUri = powerDogUri;
            this.powerDogPassword = powerDogPassword;
            refreshDataTimer = new Timer(new TimerCallback(RefreshData), null, (int)readDeviceDataInterval.TotalMilliseconds, (int)readDeviceDataInterval.TotalMilliseconds);
        }

        private void RefreshData(object? state)
        {
            PowerDog.PowerDog target = new(sensorKeys, powerDogUri, powerDogPassword);
            var data = target.ReadSensorsData();

            ConsoleHelpers.PrintSuccessMessage(20, 22, "PowerDog Updated        ");
            string consoleOutput = "";
            if (data != null)
            {
                if (data.ContainsKey("Erzeugung") && data["Erzeugung"] != null)
                {
                    PVProduction = (double)data["Erzeugung"];
                    PVProductionChanged?.Invoke(this, new DataChangedEventArgs(PVProduction));
                    consoleOutput += $"Erzeugung: {PVProduction}";
                }
                if (data.ContainsKey("Bezug") && data["Bezug"] != null)
                {
                    GridConsumption = (double)data["Bezug"];
                    GridConsumptionChanged?.Invoke(this, new DataChangedEventArgs(GridConsumption));
                    consoleOutput += $"\t Bezug: {GridConsumption}";
                }
                if (data.ContainsKey("lieferung") && data["lieferung"] != null)
                {
                    GridSupply = (double)data["lieferung"];
                    GridSupplyChanged?.Invoke(this, new DataChangedEventArgs(GridSupply));
                    consoleOutput += $"\t Lieferung: {GridSupply}";
                }
            }
            ConsoleHelpers.PrintConsoleOutput(0, 2, consoleOutput);
            Thread.Sleep(200);
            ConsoleHelpers.PrintSuccessMessage(20, 22, "PowerDog                 ");
        }
    }
}
