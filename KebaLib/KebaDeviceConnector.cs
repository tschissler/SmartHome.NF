﻿using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HelpersLib;
using SharedContracts.DataPointCollections;

namespace KebaLib
{
    public class KebaDeviceConnector
    {
        private IPAddress ipAddress;
        private int uDPPort;
        private readonly object lockObject;
        private readonly object uDPLock = new object();

        public ChargingDataPoints DataPoints { get; private set; }

        public KebaDeviceConnector(IPAddress IpAddress, int UDPPort, object lockObject = null)
        {
            ipAddress = IpAddress;
            uDPPort = UDPPort;
            this.lockObject = lockObject!=null?lockObject:new object();
            DataPoints = new ChargingDataPoints();
            // Energy is in 0.1Wh, so we need to divide by 10
            DataPoints.CarCharingActiveSession.CurrentValueCorrection = 0.1; ;
            DataPoints.CarCharingTotal.CurrentValueCorrection = 0.0001;
            DataPoints.CarLatestChargingPower.CurrentValueCorrection = 0.001;
            DataPoints.CarChargingCurrentTarget.CurrentValueCorrection = 0.001;
        }

        public void RefreshData(object? state)
        {
            var data = GetDeviceStatus();

            if (data != null)
            {
                DataPoints.CarCharingActiveSession.SetCorrectedValue(data.EnergyCurrentChargingSession);
                DataPoints.CarCharingTotal.SetCorrectedValue(data.EnergyTotal);
                DataPoints.CarLatestChargingPower.SetCorrectedValue(data.CurrentChargingPower);
                DataPoints.CarChargingCurrentTarget.SetCorrectedValue(data.TargetCurrency);
                DataPoints.CurrentVoltage.SetCorrectedValue((data.VoltagePhase1 + data.VoltagePhase2 + data.VoltagePhase3)/3);
                DataPoints.KebaStatus.CurrentValue = data.State;
            }
        }

        public string GetDeviceInformation()
        {
            return ExecuteUDPCommand("i");
        }

        public string GetDeviceReport()
        {
            return ExecuteUDPCommand("report 1");
        }

        /// <summary>
        /// Sets the charging current within 1 sec 
        /// </summary>
        /// <param name="current">Target current in mA, possible values 0; 6000 - 63000</param>
        /// <returns></returns>
        public KebaDeviceStatusData SetChargingCurrent(int current)
        {
            var success = ExecuteUDPCommand($"currtime {current} 1");
            if (success != "TCH-OK :done\n")
            {
                ConsoleHelpers.PrintErrorMessage("Setting currency failed");
            }
            Thread.Sleep(2000);
            return GetDeviceStatus();
        }

        public KebaDeviceStatusData GetDeviceStatus()
        {
            var data = new KebaDeviceStatusData();
            try
            {
                var report2 = ExecuteUDPCommand("report 2");
                var report3 = ExecuteUDPCommand("report 3");

                JObject report2Json = JObject.Parse(report2);
                JObject report3Json = JObject.Parse(report3);

                report2Json.Merge(report3Json, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });

                data = JsonConvert.DeserializeObject<KebaDeviceStatusData>(report2Json.ToString());
            }
            catch (Exception ex) 
            {
                ConsoleHelpers.PrintErrorMessage($"Error while reading device status: {ex.Message}");
            }
            return data;
        }

        private string ExecuteUDPCommand(string command)
        {
            string result = "";

            lock (lockObject)
            {
                using (UdpClient udpClient = new UdpClient(uDPPort))
                {
                    try
                    {
                        udpClient.Connect(ipAddress, uDPPort);

                        // Sends a message to the host to which you have connected.
                        byte[] sendBytes = Encoding.ASCII.GetBytes(command);

                        udpClient.Send(sendBytes, sendBytes.Length);

                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        IPEndPoint RemoteIpEndPoint = new IPEndPoint(ipAddress, 0);

                        // Blocks until a message returns on this socket from a remote host.
                        byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                        string returnData = Encoding.ASCII.GetString(receiveBytes);

#if DEBUG
                        // Uses the IPEndPoint object to determine which of these two hosts responded.
                        //Console.WriteLine("This is the message you received " +
                        //                             returnData.ToString());
                        //Console.WriteLine("This message was sent from " +
                        //                            RemoteIpEndPoint.Address.ToString() +
                        //                            " on their port number " +
                        //                            RemoteIpEndPoint.Port.ToString());
#endif
                        result = returnData.ToString();
                        udpClient.Close();
                        Thread.Sleep(200);
                    }
                    catch (Exception e)
                    {
                        ConsoleHelpers.PrintErrorMessage("Error while communicating via UDP with Keba device: " + e.Message);
                    }
                }
            }
            return result;
        }
    }
}