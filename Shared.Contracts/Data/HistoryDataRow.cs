namespace SharedContracts.Data
{
    public class HistoryDataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
    public class HistoryDataRow
    {
        public List<HistoryDataPoint> DataHistory { get; private set; }
        public int DataHistoryLength { get; set; }

        public HistoryDataRow()
        {
            DataHistory = new List<HistoryDataPoint>();
        }

        public void AddHistoryDataPoint(double dataPoint)
        {
            if (DataHistory.Count >= DataHistoryLength && DataHistoryLength > 0)
            {
                DataHistory.RemoveAt(0);
            }
            DataHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = dataPoint });
        }
    }
}
