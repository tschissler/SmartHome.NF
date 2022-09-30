namespace Smarthome.Web.Components
{
    public class RemoteDisplaySensorDataChangedEventArgs : EventArgs
    {
        public RemoteDisplaySensorDataChangedEventArgs(RemoteDisplayData data)
        {
            Data = data;
        }
        public RemoteDisplayData Data { get; }
    }

    public class RemoteDisplayData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public double Illumination { get; set; }
    }
    
    public class SensorsConnector
    {
        public delegate void RemoteDisplaySensorDataChangedEventHandler(object sender, EventArgs e);
        public event RemoteDisplaySensorDataChangedEventHandler RemoteDisplaySensorDataChanged;

        public void RemoteDisplayChanged(RemoteDisplayData sensorData)
        {
            RemoteDisplaySensorDataChanged?.Invoke(this, new RemoteDisplaySensorDataChangedEventArgs(sensorData));
        }
    }
}
