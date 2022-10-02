namespace SharedContracts.Data
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
                else
                {
                    return currentValue;
                }
                return currentValue;
            }
            set
            {
                currentValue = value;
            }
        }
        public int MaxValue { get; set; }
        public string Unit { get; set; }

        public int? CurrentValueCorrection { get; set; }
    }
}
