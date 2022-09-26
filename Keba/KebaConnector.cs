using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HelpersLib;

namespace Keba
{
    public class KebaDataChangedEventArgs : EventArgs
    {
        public KebaDataChangedEventArgs(KebaDeviceStatusData data)
        {
            Data = data;
        }
        public KebaDeviceStatusData Data { get; }
    }

    public class KebaDeviceConnector
    {
        private Timer refreshDataTimer;
        private IPAddress ipAddress;
        private int uDPPort;
        private readonly object uDPLock = new object();

        public double CurrentChargingPower { get; internal set; }

        public delegate void KebaDeviceStatusChangedEventHandler(object sender, EventArgs e);
        public event KebaDeviceStatusChangedEventHandler KebaDeviceStatusChanged;

        public KebaDeviceConnector(IPAddress IpAddress, int UDPPort, TimeSpan readDeviceDataInterval)
        {
            this.ipAddress = IpAddress;
            this.uDPPort = UDPPort;
            refreshDataTimer = new Timer(new TimerCallback(RefreshData), null, 0, (int)readDeviceDataInterval.TotalMilliseconds);
        }

        private void RefreshData(object? state)
        {
            var data = GetDeviceStatus();

            if (data != null)
            {
                if (data.CurrentChargingPower != null)
                {
                    CurrentChargingPower = (int)data.CurrentChargingPower;
                }
                KebaDeviceStatusChanged?.Invoke(this, new KebaDataChangedEventArgs(data));
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
            var report2 = ExecuteUDPCommand("report 2");
            var report3 = ExecuteUDPCommand("report 3");

            JObject report2Json = JObject.Parse(report2);
            JObject report3Json = JObject.Parse(report3);

            report2Json.Merge(report3Json, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            var data = JsonConvert.DeserializeObject<KebaDeviceStatusData>(report2Json.ToString());
            ConsoleHelpers.PrintConsoleOutput(0, 4, $"CurrentChargingPower: {data.CurrentChargingPower}\t EnergyTotal: {data.EnergyTotal}\t State: {data.State}\t TargetCurrency: {data.TargetCurrency}");
            return data;
        }

        private string ExecuteUDPCommand(string command)
        {
            string result = "";
            
            lock (uDPLock)
            {
                ConsoleHelpers.PrintSuccessMessage(0, 22, "Keba Updated        ");
                UdpClient udpClient = new UdpClient(uDPPort);
                try
                {
                    udpClient.Connect(ipAddress, uDPPort);

                    // Sends a message to the host to which you have connected.
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(command);

                    udpClient.Send(sendBytes, sendBytes.Length);

                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(ipAddress, 0);

                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
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

                    ConsoleHelpers.PrintSuccessMessage(0, 22, "Keba                ");
                }
                catch (Exception e)
                {
                    ConsoleHelpers.PrintErrorMessage("Fehler: " + e.Message);
                }
            }
            return result;
        }
    }
}