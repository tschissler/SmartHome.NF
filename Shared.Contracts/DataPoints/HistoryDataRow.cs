
namespace SharedContracts.DataPoints
{
    public static class DateTimeExtensions
    {
        public static DateTime Floor(this DateTime dateTime, TimeSpan interval)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
        }

        public static DateTime Ceiling(this DateTime dateTime, TimeSpan interval)
        {
            var overflow = dateTime.Ticks % interval.Ticks;

            return overflow == 0 ? dateTime : dateTime.AddTicks(interval.Ticks - overflow);
        }

        public static DateTime Round(this DateTime dateTime, TimeSpan interval)
        {
            var halfIntervalTicks = (interval.Ticks + 1) >> 1;

            return dateTime.AddTicks(halfIntervalTicks - ((dateTime.Ticks + halfIntervalTicks) % interval.Ticks));
        }
    }
    
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
        public List<HistoryDataPoint> AggregatedHistory { get; set; }
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

        public void AddHistoryDataPoint (double dataPoint, DateTime timeStamp)
        {
            if (DataHistory.Count >= DataHistoryLength && DataHistoryLength > 0)
            {
                DataHistory.RemoveAt(0);
            }
            DataHistory.Add(new HistoryDataPoint { Timestamp = timeStamp, Value = dataPoint });
        }

        public void AddHistoryDataPoint(double dataPoint)
        {
            if (DataHistory.Count >= DataHistoryLength && DataHistoryLength > 0)
            {
                DataHistory.RemoveAt(0);
            }
            DataHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now.Floor(new TimeSpan(0,0,1)), Value = dataPoint });

            if (AggregationHistoryLength > 0 && 
                AggregationTimeSpan.TotalMilliseconds > 0 && 
                DataHistory.Max(x => x.Timestamp) >= lastAggregationTimestamp + AggregationTimeSpan)
            {
                switch (AggregationType)
                {       
                    case AggregationType.None:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now.Floor(new TimeSpan(0, 0, 10)), Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Last().Value });
                        break;
                    case AggregationType.Average:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now.Floor(new TimeSpan(0, 0, 10)), Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Average(x => x.Value) });
                        break;
                    case AggregationType.Sum:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now.Floor(new TimeSpan(0, 0, 10)), Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Sum(x => x.Value) });
                        break;
                    case AggregationType.Min:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now.Floor(new TimeSpan(0, 0, 10)), Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Min(x => x.Value) });
                        break;
                    case AggregationType.Max:
                        AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now.Floor(new TimeSpan(0, 0, 10)), Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Max(x => x.Value) });
                        break;
                    default:
                        break;
                }
                //AggregatedHistory.Add(new HistoryDataPoint { Timestamp = DateTime.Now, Value = DataHistory.Where(i => i.Timestamp > lastAggregationTimestamp).Average(x => x.Value) });
                lastAggregationTimestamp = DateTime.Now.Floor(new TimeSpan(0, 0, 10));
                if (AggregatedHistory.Count >= AggregationHistoryLength)
                {
                    AggregatedHistory.RemoveAt(0);
                }
            }
        }
    }
}
