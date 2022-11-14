namespace SharedContracts.DataPoints
{
    public class BooleanDataPoint
    {
        private bool currentValue;

        public bool CurrentValue
        {
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
            }
        }

        public Func<bool, bool> CurrentValueCorrection;

        public string Label { get; set; }

    }
}
