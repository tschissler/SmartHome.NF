using SharedContracts.DataPointCollections;

namespace SensorDataService
{

    public class setremotedisplaydata
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double Illumination { get; set; }
    }

    public class SensorsController
    {
        public RemoteDisplayDataPoints remoteDisplayDataPoints = new();
        
        public void RemoteDisplayChanged(RemoteDisplayData sensorData)
        {
            remoteDisplayDataPoints.Temperature.SetCorrectedValue(sensorData.Temperature);
            remoteDisplayDataPoints.Humidity.SetCorrectedValue(sensorData.Humidity);
            remoteDisplayDataPoints.Pressure.SetCorrectedValue(sensorData.Pressure);
            remoteDisplayDataPoints.Illumination.SetCorrectedValue(sensorData.Illumination);
        }
    }
}
