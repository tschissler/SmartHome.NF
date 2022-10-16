using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.StorageData
{
    public class EnergyM3StorageData : ITableEntity
    {
        public string PartitionKey { get; set; } = default;
        public string RowKey { get; set; } = default;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = default;

        public double GridDemand { get; set; }
        public double GridSupply { get; set; }
        public double PVProduction { get; set; }
        public double CarCharging { get; set; }
        public int CarChargingStatus { get; set; }
    }
}
