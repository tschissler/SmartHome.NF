namespace Shared.Contracts
{
    public interface ISensorDevice
    {
        public Dictionary<string, double?> ReadSensorsData();
    }
}