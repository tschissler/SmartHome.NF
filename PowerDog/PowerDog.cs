using CookComputing.XmlRpc;
using Shared.Contracts;
using System.Globalization;

namespace PowerDog
{
    public class PowerDog : ISensorDevice
    {
        private readonly Dictionary<string, string> sensorKeys;
        private readonly Uri deviceUri;
        private readonly string password;
        private readonly XmlRpcProxy proxy;

        public PowerDog(Dictionary<string, string> sensorKeys, Uri deviceUri, string password)
        {
            this.sensorKeys = sensorKeys;
            this.deviceUri = deviceUri;
            this.password = password;
            proxy = new XmlRpcProxy();
            proxy.Url = deviceUri.ToString();
        }
        public Dictionary<string, double?> ReadSensorsData()
        {
            var result = proxy.getAllCurrentLinearValues(password);
            if (result.ErrorCode != 0)
            {
                throw new Exception(result.ErrorString);
            }
            Dictionary<string, double?> data = new();

            foreach (var sensorKey in sensorKeys)
            {
                double value;
                if (result.Reply.ContainsKey(sensorKey.Value) &&
                    ((bool)((XmlRpcStruct)result.Reply[sensorKey.Value])["Valid"]) &&
                    double.TryParse(((XmlRpcStruct)result.Reply[sensorKey.Value])["Current_Value"].ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"), out value))
                {
                    data.Add(sensorKey.Key, value);
                }
                else
                {
                    data.Add(sensorKey.Key, null);
                }
            }
            return data;
        }
    }
}