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
        public double RoomTemperature { get; set; }
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
