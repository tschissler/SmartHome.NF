using Azure.Data.Tables;
using SharedContracts.DataPointCollections;
using SharedContracts.StorageData;

namespace StorageLib
{
    public class TableStorageConnector
    {
        public static void WriteEnergyM3DataToTable(EnergyM3StorageData energyM3DataPoints)
        {
            if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString")))
            {
                throw new Exception("SmartHomeStorageConnectionString is not set");
            }
            TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString"));
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: "SmartHomeEnergieM3");

            tableClient.CreateIfNotExistsAsync().Wait();

            tableClient.AddEntityAsync(energyM3DataPoints).Wait();
        }
    }
}