using FluentAssertions;
using SharedContracts.DataPointCollections;
using Newtonsoft.Json;
using SharedContracts.DataPoints;

namespace ServicesTests
{
    [TestClass]
    public class ReadHistoryDataTests
    {
        [TestMethod]
        public void ReadPVHistoryData()
        {
            using (HttpClient Http = new HttpClient())
            {
                var jsonString = Http.GetStringAsync($"http://smarthomepi:5005/readconsumptiondata").Result;
                var dataPoints = JsonConvert.DeserializeObject<ConsumptionDataPoints>(jsonString);
                //var dataPoints = System.Text.Json.JsonSerializer.Deserialize<ConsumptionDataPoints>(jsonString);
                dataPoints.PowerPhase1.History.DataHistory.RemoveAt(0);
                dataPoints.PowerPhase1.History.DataHistory.First().Timestamp.Should().BeBefore(dataPoints.PowerPhase1.History.DataHistory.Last().Timestamp);
            }
        }
    }
}