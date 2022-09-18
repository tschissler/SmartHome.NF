namespace Smarthome.Web.Data
{
    public class IntegerDataPoint
    {
        private int currentValue;

        public int CurrentValue { 
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
        public int MaxValue { get; set; }
        public string Unit { get; set; }

        public Func<int, int> CurrentValueCorrection;
    }
}
