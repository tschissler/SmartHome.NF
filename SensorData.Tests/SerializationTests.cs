using FluentAssertions;
using SensorDataService;
using System.Text.Json;

namespace SensorData.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void DeserializeConsumptionDataTest()
        {
            string jsonString = "{\"GasTriggerTimestamps\":[],\"PowerTriggerTimestamps\":[\"2022-10-11T06:50:56.6884400Z\",\"2022-10-11T06:50:56.6922310Z\",\"2022-10-11T06:50:56.6958030Z\",\"2022-10-11T06:50:56.6993840Z\",\"2022-10-11T06:50:56.7053750Z\",\"2022-10-11T06:50:56.8322590Z\",\"2022-10-11T06:50:56.9527190Z\",\"2022-10-11T06:50:57.1730390Z\",\"2022-10-11T06:50:57.2302950Z\",\"2022-10-11T06:50:57.3580110Z\",\"2022-10-11T06:50:57.6414540Z\",\"2022-10-11T06:50:57.8184910Z\",\"2022-10-11T06:50:57.8222920Z\",\"2022-10-11T06:50:57.8307180Z\",\"2022-10-11T06:50:57.8368900Z\",\"2022-10-11T06:50:57.8407880Z\",\"2022-10-11T06:50:57.8476790Z\",\"2022-10-11T06:50:57.8561720Z\",\"2022-10-11T06:50:58.2133980Z\",\"2022-10-11T06:50:58.4182180Z\",\"2022-10-11T06:50:58.7860500Z\"]}";
            var data = JsonSerializer.Deserialize<ConsumptionData>(jsonString);
            data.GasTriggerTimestamps.Should().NotBeNull();
            data.PowerTriggerTimestamps.Should().NotBeNull();
            data.PowerTriggerTimestamps[0].Should().Be(new DateTime(2022, 10, 11, 6, 50, 56, 688).AddTicks(4400));
        }
    }
}