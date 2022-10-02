namespace SharedContracts.Data
{
    public class DecimalDataPoint
    {
        private double currentValue;

        public double CurrentValue
        {
            get
            {
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
        public double? CurrentValueCorrection { get; set; }
        public string Label { get; set; }

        public DecimalDataPoint()
        {
            History = new HistoryDataRow();
        }

        public void SetCorrectedValue(double value)
        {
            if (CurrentValueCorrection != null)
            {
                CurrentValue = value * CurrentValueCorrection.Value;
            }
            else
            {
                CurrentValue = value;
            }
        }
    }
}
