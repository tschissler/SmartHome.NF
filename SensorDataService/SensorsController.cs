using SharedContracts.DataPointCollections;
using System.Collections;

namespace SensorDataService
{
    public class RemoteDisplayData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double Illumination { get; set; }
    }

    public class ConsumptionData
    {
        public List<DateTime> GasTriggerTimestamps { get; set; }
        public List<DateTime> PowerTriggerTimestamps { get; set; }
    }

    public class SensorsController
    {
        public object RemoteDisplayLockObject = new object();
        public object ConsumptionLockObject = new object();
        public RemoteDisplayDataPoints remoteDisplayDataPoints = new();
        
        public void RemoteDisplayChanged(RemoteDisplayData sensorData)
        {
            lock (RemoteDisplayLockObject)
            {
                remoteDisplayDataPoints.Temperature.SetCorrectedValue(sensorData.Temperature);
                remoteDisplayDataPoints.Humidity.SetCorrectedValue(sensorData.Humidity);
                remoteDisplayDataPoints.Pressure.SetCorrectedValue(sensorData.Pressure);
                remoteDisplayDataPoints.Illumination.SetCorrectedValue(sensorData.Illumination);
            }
        }

        public void ConsumptionChanged(ConsumptionData sensorData)
        {
            lock (ConsumptionLockObject)
            {
                int i = 1;
            }
        }
    }
}
