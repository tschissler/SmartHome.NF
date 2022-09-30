namespace Smarthome.Web.Data
{
    public class DecimalDataPoint
    {
        private double currentValue;

        public double CurrentValue { 
            get
            {
                if (CurrentValueCorrection != null)
                {
                    return CurrentValueCorrection(currentValue);
                }
                else
                {
                    return currentValue;
                }
                return currentValue;
            }
            set
            {
                currentValue = value;
                LastUpdate = DateTime.Now;
                History.AddHistoryDataPoint(value);
            }
        }
        public double MaxValue { get; set; }
        public string Unit { get; set; }
        public int DecimalDigits { get; set; }
        public DateTime LastUpdate { get; private set; }

        public HistoryDataRow History { get; set; }

        public Func<double, double> CurrentValueCorrection;

        public DecimalDataPoint()
        {
            History = new HistoryDataRow();
        }
    }
}
