using Microsoft.Azure.Cosmos.Table;

namespace SmartHome.Web.Client.DataContracts
{
    public class MeasureData : TableEntity
    {
        public string PartitionKey { get; set; }
        public DateTime DeviceUTCTime { get; set; }
        public int GasPulse { get; set; }
        public double KellerHumidity { get; set; }
        public double KellerTemp { get; set; }
        public double ZisterneLevel { get; set; }
    }
}
