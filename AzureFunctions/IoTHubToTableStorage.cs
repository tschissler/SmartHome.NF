using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctions
{
    public static class IoTHubToTableStorage
    {
        private static CloudTable _outputTable = CloudTableHelper.GetCloudTable("IoTDataM1", Secrets.AzureStorageConnectionString);

        [FunctionName("IoTHubToTableStorage")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IotHubEndpoint")]EventData message, ILogger log)
        {
            var messageStr = Encoding.UTF8.GetString(message.Body.ToArray());
            messageStr = messageStr.Replace("nan", "\"nan\"");
            log.LogInformation($"IoT Hub trigger function processed a message: {messageStr}");
            var data = JObject.Parse(messageStr);

            var dynamicTableEntity = new DynamicTableEntity();

            foreach (KeyValuePair<string, JToken> keyValuePair in data)
            {
                if (keyValuePair.Key.Equals("deviceId"))
                {
                    dynamicTableEntity.PartitionKey = keyValuePair.Value.ToString();

                }
                else if (keyValuePair.Key.Equals("messageId"))
                {
                    dynamicTableEntity.RowKey = keyValuePair.Value.ToString();
                }
                else
                {
                    dynamicTableEntity.Properties.Add(keyValuePair.Key, EntityProperty.CreateEntityPropertyFromObject(keyValuePair.Value));
                }
            }

            if (dynamicTableEntity.PartitionKey == null)
            {
                dynamicTableEntity.PartitionKey = "Default";
            }

            if (dynamicTableEntity.RowKey == null)
            {
                dynamicTableEntity.RowKey = Guid.NewGuid().ToString();
            }

            var tableOperation = TableOperation.InsertOrMerge(dynamicTableEntity);
            _outputTable.ExecuteAsync(tableOperation).Wait();
        }
    }
    public class CloudTableHelper
    {
        public static CloudTable GetCloudTable(string tableName, string storageConnectionString)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to a table.
            CloudTable table = tableClient.GetTableReference(tableName);

            // Create the table if it doesn't already exist
            table.CreateIfNotExistsAsync().Wait();

            return table;
        }

    }
}