using HelpersLib;
using Newtonsoft.Json;
using SharedContracts.DataPointCollections;
using SharedContracts.StorageData;
using StorageLib;

namespace StorageService
{
    public class StorageConnector
    {
        private Timer highFrequencyTimer;
        private TimeSpan highFrequency = TimeSpan.FromSeconds(2);
        private Timer lowFrequencyTimer;
        private TimeSpan lowFrequency = TimeSpan.FromMinutes(10);

        public StorageConnector()
        {
            if (Environment.GetEnvironmentVariable("WRITE_TABLE_TO_TABLESTORAGE") != null && Environment.GetEnvironmentVariable("WRITE_TABLE_TO_TABLESTORAGE").ToLower() == "false")
            {
                ConsoleHelpers.PrintInformation("WRITE_TABLE_TO_TABLESTORAGE is set to false, so we will not write data to the cloud");
            }
            else
            {
                highFrequencyTimer = new Timer(WriteHighFrequencyData, null, 0, (int)highFrequency.TotalMilliseconds);
                lowFrequencyTimer = new Timer(WriteLowFrequencyData, null, 0, (int)lowFrequency.TotalMilliseconds);
            }
        }

        private void WriteLowFrequencyData(object? state)
        {
            RemoteDisplayDataPoints remoteDisplayDataPoints;
            try
            {
                using (HttpClient Http = new HttpClient())
                {
                    var jsonString = Http.GetStringAsync($"http://localhost:5005/readremotedisplaydata").Result;
                    remoteDisplayDataPoints = JsonConvert.DeserializeObject<RemoteDisplayDataPoints>(jsonString);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage("Failed to read data from SensorDataService, Error: " + ex.Message);
                throw new Exception("Failed to read data from SensorDataService", ex);
            }
            try
            {
                var storageData = new List<LowFrequencyData>();
                
                //foreach (var data in remoteDisplayDataPoints.)
                //var timeStamp = DateTime.UtcNow;
                //LowFrequencyData storageEntity = new()
                //{
                //    RowKey = timeStamp.ToString("yyyyMMddHHmmss"),
                //    PartitionKey = timeStamp.ToString("yyyyMMdd"),
                //    Timestamp = timeStamp,
                //};
                //TableStorageConnector.WriteLowFrequencyDataToTable(storageData);
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Error while storing data: {ex.Message}");
            }
        }

        private void WriteHighFrequencyData(object? state)
        {
            throw new NotImplementedException();
        }
    }
}
