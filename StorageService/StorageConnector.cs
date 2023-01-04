using Azure.Data.Tables;
using Azure;
using HelpersLib;
using Newtonsoft.Json;
using SharedContracts.DataPointCollections;
using SharedContracts.RestDataPoints;
using SharedContracts.StorageData;
using StorageLib;

namespace StorageService
{
    public class StorageConnector
    {
        public static async void AddPVM3Data(List<PVM3RestDataPoint> m3PVRestDataPoints)
        {
            ConsoleHelpers.PrintInformation("AddPVM3Data() called");
            try
            {
                List<TableTransactionAction> addEntitiesBatch = new List<TableTransactionAction>();

                foreach (var data in m3PVRestDataPoints)
                {
                    PVM3StorageData storageEntity = new()
                    {
                        RowKey = data.TimeStamp.ToString("yyyyMMddHHmmss"),
                        PartitionKey = data.TimeStamp.ToString("yyyyMMdd"),
                        Timestamp = data.TimeStamp,
                        GridDemand = data.GridDemand,
                        GridSupply = data.GridSupply,
                        PVProduction = data.PVProduction
                    };
                    addEntitiesBatch.Add(new TableTransactionAction(TableTransactionActionType.Add, storageEntity));
                }
                await TableStorageConnector.BatchWriteDataToTable(addEntitiesBatch, "SmartHomePVM3", Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString"));
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Error while storing data: {ex.Message}");
            }
        }

        public static async Task<List<PVM3RestDataPoint>> ReadPVM3Data(DateTime timeStampFrom)
        {
            try
            {
                var data = await TableStorageConnector.ReadLatestPVM3Data(timeStampFrom, "SmartHomePVM3", Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString"));

                var restDataPoints = new List<PVM3RestDataPoint>();

                foreach (var item in data)
                {
                    restDataPoints.Add(new PVM3RestDataPoint
                    {
                        GridDemand = item.GridDemand,
                        GridSupply = item.GridSupply,
                        PVProduction = item.PVProduction,
                        TimeStamp = item.Timestamp.Value.LocalDateTime
                    });
                }
                return restDataPoints;
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Error while reading PV M3 data: {ex.Message}");
                throw;
            }
        }
    }
}
