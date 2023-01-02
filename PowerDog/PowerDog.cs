using CookComputing.XmlRpc;
using HelpersLib;
using SharedContracts.DataPointCollections;
using SharedContracts.RestDataPoints;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace PowerDogLib
{
    public class PowerDog
    {
        private readonly Dictionary<string, string> sensorKeys;
        private readonly string password;
        private readonly XmlRpcProxy proxy;

        public object Lockobject = new object();

        public PVDataPoints LocalDataPoints { get; private set; }

        private List<PVM3RestDataPoint> cloudDataCache;

        public PowerDog(Dictionary<string, string> sensorKeys, Uri deviceUri, string password)
        {
            this.sensorKeys = sensorKeys;
            this.password = password;
            proxy = new XmlRpcProxy();
            proxy.Url = deviceUri.ToString();
            LocalDataPoints = new PVDataPoints();
        }

        public List<PVM3RestDataPoint> ReadCloudDataCache()
        {
            var returndata = new List<PVM3RestDataPoint>();
            if (Monitor.TryEnter(Lockobject, 1000))
            {
                try
                {
                    returndata = cloudDataCache.ToList();
                    cloudDataCache.Clear();
                }
                finally
                {
                    Monitor.Exit(Lockobject);
                }
            }

            return returndata;
        }

        public void ReadSensorsData(object? state)
        {
            if (Monitor.TryEnter(Lockobject, 1000))
            {
                try
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
                    double production = ParseSensorValue(result.Reply, sensorKeys["Erzeugung"]);
                    double gridSupply = ParseSensorValue(result.Reply, sensorKeys["Lieferung"]);
                    double gridDemand = ParseSensorValue(result.Reply, sensorKeys["Bezug"]);

                    LocalDataPoints.PVProduction.SetCorrectedValue(production);
                    LocalDataPoints.GridSupply.SetCorrectedValue(gridSupply);
                    LocalDataPoints.GridDemand.SetCorrectedValue(gridDemand);

                    cloudDataCache.Add(new PVM3RestDataPoint
                    {
                        GridDemand = gridDemand,
                        GridSupply = gridSupply,
                        PVProduction = production,
                        TimeStamp = DateTime.Now
                    });
                }
                finally
                {
                    Monitor.Exit(Lockobject);
                }
            }
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