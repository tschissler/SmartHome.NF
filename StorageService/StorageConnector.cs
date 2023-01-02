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
                TableStorageConnector.BatchWriteDataToTable(addEntitiesBatch, "SmartHome_PVM3");

            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Error while storing data: {ex.Message}");
            }
        }
    }
}
