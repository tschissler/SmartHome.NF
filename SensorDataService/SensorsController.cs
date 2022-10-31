﻿using SharedContracts.DataPointCollections;
using ShellyLib;
using System.Collections;
using System.Net;

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
        public RemoteDisplayDataPoints remoteDisplayDataPoints = new();
        public ConsumptionDataPoints consumptionDataPoints = new();

        private DateTime previousPowerTimeStamp = DateTime.MinValue;
        private DateTime previousGasTimeStamp = DateTime.MinValue;

        private TimeSpan gatherSensorDataInterval = TimeSpan.FromSeconds(2);
        private Timer gatherSensorDataTimer;

        public object LockObject = new object();

        public SensorsController()
        {
            gatherSensorDataTimer = new Timer(GatherSensorData, null, 0, (int)gatherSensorDataInterval.TotalMilliseconds);
        }

        private void GatherSensorData(object? state)
        {
            lock (LockObject)
            {
                GC.Collect();
                var gcInfo = GC.GetGCMemoryInfo();
                Console.WriteLine($"Heap: {gcInfo.HeapSizeBytes} \t| Load Bytes: {gcInfo.MemoryLoadBytes} \t| Available: {gcInfo.TotalAvailableMemoryBytes} \t | Commited: {gcInfo.TotalCommittedBytes}");

                consumptionDataPoints.PowerDevice1.SetCorrectedValue(ShellyConnector.ReadPlugPower(new IPAddress(new byte[] { 192, 168, 178, 177 })));
                consumptionDataPoints.PowerDevice2.SetCorrectedValue(ShellyConnector.ReadPlugPower(new IPAddress(new byte[] { 192, 168, 178, 178 })));
                var em3Data = ShellyConnector.Read3EMPower(new IPAddress(new byte[] { 192, 168, 178, 179 }));
                consumptionDataPoints.PowerPhase1.SetCorrectedValue(em3Data[0]);
                consumptionDataPoints.PowerPhase2.SetCorrectedValue(em3Data[1]);
                consumptionDataPoints.PowerPhase3.SetCorrectedValue(em3Data[2]);
            }
        }

        public void RemoteDisplayChanged(RemoteDisplayData sensorData)
        {
            lock (LockObject)
            {
                remoteDisplayDataPoints.Temperature.SetCorrectedValue(sensorData.Temperature);
                remoteDisplayDataPoints.Humidity.SetCorrectedValue(sensorData.Humidity);
                remoteDisplayDataPoints.Pressure.SetCorrectedValue(sensorData.Pressure);
                remoteDisplayDataPoints.Illumination.SetCorrectedValue(sensorData.Illumination);
            }
        }

        public void ConsumptionChanged(ConsumptionData sensorData)
        {
            lock (LockObject)
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

                consumptionDataPoints.WaterLevel.SetCorrectedValue(sensorData.WaterLevel);
                consumptionDataPoints.Temperature.SetCorrectedValue(sensorData.Temperature);
                consumptionDataPoints.Humidity.SetCorrectedValue(sensorData.Humidity);
            }
        }
    }
}
