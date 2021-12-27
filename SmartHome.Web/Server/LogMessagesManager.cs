using Azure.Storage.Queues;
using Microsoft.Azure.Cosmos.Table;

namespace SmartHome.Web.Server
{
    public class LogMessagesManager
    {
        public static void WriteLogMessageToTableStorage(string logMessage)
        {
            QueueClient queueClient = new QueueClient(Secrets.AzureStorageConnectionString, "logmessages");
            // Create the queue
            queueClient.CreateIfNotExists();
            if (queueClient.Exists())
            {
                // Send a message to the queue
                queueClient.SendMessage(logMessage, timeToLive: new TimeSpan(30,0,0,0));
            }
        }

        private static Queue<string> _logMessages = new Queue<string>();

        public static void Enqueue(string message)
        {
            if (_logMessages.Count > 1000)
            {
                _logMessages.Dequeue();
            }
            _logMessages.Enqueue(message);
            WriteLogMessageToTableStorage(message);
        }

        public static string[] ReadLogMessages()
        {
            return _logMessages.ToArray();
        }
    }
}
