using CookComputing.XmlRpc;
using HelpersLib;
using SharedContracts.DataPointCollections;
using System.Globalization;

namespace PowerDogLib
{
    public class PowerDog
    {
        private readonly Dictionary<string, string> sensorKeys;
        private readonly string password;
        private readonly XmlRpcProxy proxy;

        public PVDataPoints DataPoints { get; private set; }

        public PowerDog(Dictionary<string, string> sensorKeys, Uri deviceUri, string password)
        {
            this.sensorKeys = sensorKeys;
            this.password = password;
            proxy = new XmlRpcProxy();
            proxy.Url = deviceUri.ToString();
            DataPoints = new PVDataPoints();
        }

        public void ReadSensorsData(object? state)
        {
            var result = proxy.getAllCurrentLinearValues(password);
            if (result.ErrorCode != 0)
            {
                ConsoleHelpers.PrintErrorMessage($"Error reading data from PowerDog: {result.ErrorString}");
                return;
            }

            if (result.Reply == null)
            {
                ConsoleHelpers.PrintErrorMessage("Reply is empty, communication with PowerDog failed");
                return;
            }

            DataPoints.PVProduction.SetCorrectedValue(ParseSensorValue(result.Reply, sensorKeys["Erzeugung"]));
            DataPoints.GridSupply.SetCorrectedValue(ParseSensorValue(result.Reply, sensorKeys["lieferung"]));
            DataPoints.GridDemand.SetCorrectedValue(ParseSensorValue(result.Reply, sensorKeys["Bezug"]));
        }

        private double ParseSensorValue(XmlRpcStruct reply, string sensorKey)
        {
            double value;

            if (reply.ContainsKey(sensorKey) &&
                (bool)((XmlRpcStruct)reply[sensorKey])["Valid"] &&
                double.TryParse(((XmlRpcStruct)reply[sensorKey])["Current_Value"].ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"), out value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }
    }
}