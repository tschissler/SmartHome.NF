using Azure;
using Azure.Data.Tables;

namespace SharedContracts.StorageData
{
    public class LowFrequencyData : ITableEntity
    { 
        public string PartitionKey { get; set;}
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public double TemperatureRemoteDisplay { get; set; }
        public double BarometricPressureRemoteDisplay { get; set; }
        public double HumidityRemoteDisplay { get; set; }
        public double BrightnessRemoteDisplay { get; set; }
        public double CisternLevel { get; set; }
        public double TemperatureBasement { get; set; }
        public double HumidityBasement { get; set; }
    }
}
