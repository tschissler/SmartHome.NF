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
        public double WaterLevel { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }

    public class SensorsController
    {
        public object RemoteDisplayLockObject = new object();
        public object ConsumptionLockObject = new object();
        public RemoteDisplayDataPoints remoteDisplayDataPoints = new();
        public ConsumptionDataPoints consumptionDataPoints = new();

        private DateTime previousPowerTimeStamp = DateTime.MinValue;
        private DateTime previousGasTimeStamp = DateTime.MinValue;

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
                if (sensorData.PowerTriggerTimestamps != null && sensorData.PowerTriggerTimestamps.Count > 0)
                {
                    foreach (var timestamp in sensorData.PowerTriggerTimestamps)
                    {
                        if (previousPowerTimeStamp != DateTime.MinValue)
                        {
                            // If time is too short, it is not a new impulse but contact is bouncing
                            if ((timestamp - previousPowerTimeStamp).TotalSeconds < 1)
                            {
                                break;
                            }
                            consumptionDataPoints.Power.SetCorrectedValue(48000 / (timestamp - previousPowerTimeStamp).TotalSeconds);
                        }
                        previousPowerTimeStamp = timestamp;
                    }
                }

                if (sensorData.GasTriggerTimestamps != null && sensorData.GasTriggerTimestamps.Count > 0)
                {
                    foreach (var timestamp in sensorData.GasTriggerTimestamps)
                    {
                        if (previousGasTimeStamp != DateTime.MinValue)
                        {
                            // If time is too short, it is not a new impulse but contact is bouncing
                            if ((timestamp - previousGasTimeStamp).TotalSeconds < 1)
                            {
                                break;
                            }
                            consumptionDataPoints.Gas.SetCorrectedValue(36 / (timestamp - previousGasTimeStamp).TotalSeconds);
                        }
                        previousGasTimeStamp = timestamp;
                    }
                }
            }
        }
    }
}
