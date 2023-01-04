using Azure;
using Azure.Data.Tables;
using SharedContracts.DataPointCollections;
using SharedContracts.StorageData;

namespace StorageLib
{
    public class TableStorageConnector
    {
        public const string ConnectionStringEnvironmentVariable = "SmartHomeStorageConnectionString";
        
        //static TableStorageConnector()
        //{
        //    if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable)))
        //    {
        //        throw new Exception($"{ConnectionStringEnvironmentVariable} is not set");
        //    }
        //}
        
        public static async Task BatchWriteDataToTable(List<TableTransactionAction> data, string tableName, string connectionString)
        {
            TableServiceClient tableServiceClient = new TableServiceClient(connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);

            tableClient.CreateIfNotExistsAsync().Wait();

            Response<IReadOnlyList<Response>> response = await tableClient.SubmitTransactionAsync(data).ConfigureAwait(false);

            if (response.Value.Count != data.Count)
            {
                throw new Exception("Not all data was written to the table");
            }
        }

        public static async Task<Pageable<PVM3StorageData>> ReadLatestPVM3Data(DateTime fromTimeStamp, string tableName, string connectionString)
        {
            TableServiceClient tableServiceClient = new TableServiceClient(connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);

            return tableClient.Query<PVM3StorageData>($"Timestamp ge datetime'{ fromTimeStamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ")}'");
        }
        
        public static int CountDataTableEntriesPerPartitionKey(string tableName, string partitionKey, string connectionString)
        {
            TableServiceClient tableServiceClient = new TableServiceClient(connectionString);
            TableClient tableClient = tableServiceClient.GetTableClient(tableName: tableName);

            return tableClient.Query<TableEntity>($"PartitionKey eq '{partitionKey}'").Count();
        }

        //public static async void WritePVM3DataToTable(List<PVM3StorageData> pvM3DataPoints)
        //{
        //    if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString")))
        //    {
        //        throw new Exception("SmartHomeStorageConnectionString is not set");
        //    }
        //    TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString"));
        //    TableClient tableClient = tableServiceClient.GetTableClient(tableName: "SmartHomePVM3");

        //    tableClient.CreateIfNotExistsAsync().Wait();

        //    List<TableTransactionAction> addEntitiesBatch = new List<TableTransactionAction>();
        //    addEntitiesBatch.AddRange(pvM3DataPoints.Select(e => new TableTransactionAction(TableTransactionActionType.Add, e)));
        //    Response<IReadOnlyList<Response>> response = await tableClient.SubmitTransactionAsync(addEntitiesBatch).ConfigureAwait(false);

        //    //tableClient.AddEntityAsync(pvM3DataPoints).Wait();
        //}

        //public static async void WriteLowFrequencyDataToTable(List<LowFrequencyData> lowFrequencyData)
        //{
        //    TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString"));
        //    TableClient tableClient = tableServiceClient.GetTableClient(tableName: "LowFrequencyData");

        //    if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString")))
        //    {
        //        throw new Exception("SmartHomeStorageConnectionString is not set");
        //    }

        //    tableClient.CreateIfNotExistsAsync().Wait();

        //    List<TableTransactionAction> addEntitiesBatch = new List<TableTransactionAction>();
        //    addEntitiesBatch.AddRange(lowFrequencyData.Select(e => new TableTransactionAction(TableTransactionActionType.Add, e)));
        //    Response<IReadOnlyList<Response>> response = await tableClient.SubmitTransactionAsync(addEntitiesBatch).ConfigureAwait(false);
        //}
    }
}