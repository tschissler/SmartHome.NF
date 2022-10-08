namespace SharedContracts.DataPoints
{
    public class IntegerDataPoint
    {
        private int currentValue;

        public int CurrentValue
        {
            get
            {
                if (CurrentValueCorrection != null)
                {
                    return currentValue * CurrentValueCorrection.Value;
                }
                return currentValue;
            }
            set
            {
                LastUpdate = DateTime.Now;
                if (value != currentValue)
                {
                    LastChanged = DateTime.Now;
                }
                currentValue = value;
            }
        }
        public int MaxValue { get; set; }
        public string Unit { get; set; }
        public string Label { get; set; }
        public DateTime LastUpdate { get; private set; }
        public DateTime LastChanged { get; private set; }


        public int? CurrentValueCorrection { get; set; }
    }
}
