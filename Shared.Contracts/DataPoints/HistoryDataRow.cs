namespace SharedContracts.DataPoints
{
    public class HistoryDataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

    public enum AggregationType
    {
        None,
        Average,
        Sum,
        Min,
        Max
    }

    public class HistoryDataRow
    {
        public List<HistoryDataPoint> DataHistory { get; private set; }
        public List<HistoryDataPoint> AggregatedHistory { get; private set; }
        public int DataHistoryLength { get; set; }
        public int AggregationHistoryLength { get; set; }
        public TimeSpan AggregationTimeSpan { get; set; }
        public AggregationType AggregationType { get; set; }

        private DateTime lastAggregationTimestamp;

        public HistoryDataRow()
        {
            DataHistory = new List<HistoryDataPoint>();
            AggregatedHistory = new List<HistoryDataPoint>();
            lastAggregationTimestamp = DateTime.Now;
        }

        public void AddHistoryDataPoint(double dataPoint)
        {
            if (DataHistory.Count >= DataHistoryLength && DataHistoryLength > 0)
            {
                DataHistory.RemoveAt(0);
            }
            DataHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = dataPoint });

            if (AggregationHistoryLength > 0 && 
                AggregationTimeSpan.TotalMilliseconds > 0 && 
                DataHistory.Max(x => x.Timestamp) >= lastAggregationTimestamp + AggregationTimeSpan)
            {
                switch (AggregationType)
                {       
                    case AggregationType.None:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Last().Value });
                        break;
                    case AggregationType.Average:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Average(x => x.Value) });
                        break;
                    case AggregationType.Sum:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Sum(x => x.Value) });
                        break;
                    case AggregationType.Min:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Min(x => x.Value) });
                        break;
                    case AggregationType.Max:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Max(x => x.Value) });
                        break;
                    default:
                        break;
                }
                AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Average(x => x.Value) });
                lastAggregationTimestamp = DateTime.Now;
                if (AggregatedHistory.Count >= AggregationHistoryLength)
                {
                    AggregatedHistory.RemoveAt(0);
                }
            }
        }
    }
}
