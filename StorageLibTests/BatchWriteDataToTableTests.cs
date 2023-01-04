using Azure.Data.Tables;
using FluentAssertions;
using SharedContracts.StorageData;
using StorageLib;

namespace StorageLibTests
{
    [TestClass]
    public class BatchWriteDataToTableTests
    {
        public const string TestTableName = "UnitTests";
        
        [TestMethod]
        public void WriteTestDataToTable()
        {           
            List<TableTransactionAction> testdata = new();
            string key = Guid.NewGuid().ToString();
            int count = 3;
            
            var timeStamp = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                PVM3StorageData storageEntity = new()
                {
                    RowKey = timeStamp.ToString("yyyyMMddHHmmss") + "-" + i,
                    PartitionKey = key,
                    Timestamp = timeStamp,
                    GridDemand = new Random().Next(1000),
                    GridSupply = new Random().Next(1000),
                    PVProduction = new Random().Next(1000)
                };
                testdata.Add(new TableTransactionAction(TableTransactionActionType.Add, storageEntity));
            }

            TableStorageConnector.BatchWriteDataToTable(testdata, TestTableName, Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString_Test")).Wait();

            TableStorageConnector.CountDataTableEntriesPerPartitionKey(TestTableName, key, Environment.GetEnvironmentVariable("SmartHomeStorageConnectionString_Test")).Should().Be(count);
        }

        [TestMethod]
        public void CheckForExceptionWhenConnectionStringMissing()
        {
            List<TableTransactionAction> testdata = new();
            string key = Guid.NewGuid().ToString();
            int count = 3;

            var timeStamp = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                PVM3StorageData storageEntity = new()
                {
                    RowKey = timeStamp.ToString("yyyyMMddHHmmss") + "-" + i,
                    PartitionKey = key,
                    Timestamp = timeStamp,
                    GridDemand = new Random().Next(1000),
                    GridSupply = new Random().Next(1000),
                    PVProduction = new Random().Next(1000)
                };
                testdata.Add(new TableTransactionAction(TableTransactionActionType.Add, storageEntity));
            }

            Action act = () => TableStorageConnector.BatchWriteDataToTable(testdata, TestTableName, "").Wait();
            act.Should().Throw<Exception>();
        }
    }
}