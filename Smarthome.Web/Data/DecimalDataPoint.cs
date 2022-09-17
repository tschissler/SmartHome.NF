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
            }
        }
        public double MaxValue { get; set; }
        public string Unit { get; set; }

        public Func<double, double> CurrentValueCorrection;
    }
}
