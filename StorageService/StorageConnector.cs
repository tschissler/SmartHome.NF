namespace StorageService
{
    public class StorageConnector
    {
        private Timer highFrequencyTimer;
        private TimeSpan highFrequency = TimeSpan.FromSeconds(2);
        private Timer lowFrequencyTimer;
        private TimeSpan lowFrequency = TimeSpan.FromMinutes(10);

        public StorageConnector()
        {
            highFrequencyTimer = new Timer(WriteHighFrequencyData, null, 0, (int)highFrequency.TotalMilliseconds);
            lowFrequencyTimer = new Timer(WriteLowFrequencyData, null, 0, (int)lowFrequency.TotalMilliseconds);
        }

        private void WriteLowFrequencyData(object? state)
        {
            throw new NotImplementedException();
        }

        private void WriteHighFrequencyData(object? state)
        {
            throw new NotImplementedException();
        }
    }
}
