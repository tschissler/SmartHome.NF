﻿using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HelpersLib;
using SharedContracts.DataPointCollections;
using SharedContracts.DataPoints;

namespace KebaLib
{
    public class KebaDeviceConnector : IDisposable
    {
        private IPAddress ipAddress;
        private int uDPPort;
        private readonly object lockObject;
        //private UdpClient udpClient;
        private double previousChargingCurrencyWrittenToDevice = -99;
        private Timer updateTimer;
        private TimeSpan updateTimeSpan = TimeSpan.FromSeconds(2);

        public ChargingDataPoints DataPoints { get; private set; }

        public KebaDeviceConnector(IPAddress IpAddress, int UDPPort, object lockObject = null)
        {
            ipAddress = IpAddress;
            uDPPort = UDPPort;
            //udpClient = new UdpClient(uDPPort);
            //udpClient.Connect(ipAddress, uDPPort);
            this.lockObject = lockObject != null ? lockObject : new object();
            DataPoints = new ChargingDataPoints();
            // Energy is in 0.1Wh, so we need to divide by 10
            DataPoints.ConsumptionActiveSession.CurrentValueCorrection = 0.1; ;
            DataPoints.CharingOverallTotal.CurrentValueCorrection = 0.0001;
            DataPoints.CurrentChargingPower.CurrentValueCorrection = 0.001;
            DataPoints.EffectiveMaximumChargingCurrency.CurrentValueCorrection = 0.001;

            updateTimer = new Timer(RefreshData, null, 0, (int)updateTimeSpan.TotalMilliseconds);
        }

        public void SetChargingCurrent(double current)
        {
            DataPoints.AdjustedCharingCurrency.CurrentValue = current;
        }

        /// <summary>
        /// Reads data from the Keba device and updates the DataPoints and writes target current to the device.
        /// </summary>
        /// <remarks>
        /// As the Keba documentation states there should be a 2 second delay between sending commands to the device, this method should not be called more frequently.
        /// </remarks>
        /// <param name="state"></param>
        internal void RefreshData(object? state)
        {
            KebaDeviceStatusData data = null;
            try
            {
                data = GetDeviceStatus();
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Failed to read data from Keba device, Error: {ex.Message}");
            }

            if (data != null)
            {
                DataPoints.CurrentChargingPower.SetCorrectedValue(data.CurrentChargingPower);
                DataPoints.EffectiveMaximumChargingCurrency.SetCorrectedValue(data.MaxCurrency);
                DataPoints.CurrentVoltage.SetCorrectedValue((data.VoltagePhase1 + data.VoltagePhase2 + data.VoltagePhase3) / 3);
                DataPoints.ConsumptionActiveSession.SetCorrectedValue(data.EnergyCurrentChargingSession);
                DataPoints.CharingOverallTotal.SetCorrectedValue(data.EnergyTotal);
                DataPoints.KebaStatus.CurrentValue = data.State;


                //DataPoints.CurrentChargingPower = data.CurrentChargingPower;
                //DataPoints.EffectiveMaximumChargingCurrency = new() { Label = "Aktuell eingestellter Strom", Unit = "A", MaxValue = 16, DecimalDigits = 1 };
                //DataPoints.CarChargingManualCurrency = new() { Unit = "mA", MaxValue = 16000 };

                ConsoleHelpers.PrintMessage($"Active Session: {DataPoints.ConsumptionActiveSession.CurrentValue}Wh, " +
                    $"Total: {DataPoints.CharingOverallTotal.CurrentValue}kWh, "+
                    $"Power: {DataPoints.CurrentChargingPower.CurrentValue}kW, " +
                    $"Max: {DataPoints.EffectiveMaximumChargingCurrency.CurrentValue}kW, " +
                    $"State: {DataPoints.KebaStatus.CurrentValue}");
            }

            if (previousChargingCurrencyWrittenToDevice != DataPoints.AdjustedCharingCurrency.CurrentValue)
            {
                try
                {
                    previousChargingCurrencyWrittenToDevice = DataPoints.AdjustedCharingCurrency.CurrentValue;
                    WriteChargingCurrentToDevice((int)(DataPoints.AdjustedCharingCurrency.CurrentValue * 1000));
                    ConsoleHelpers.PrintMessage($"Wrote {DataPoints.AdjustedCharingCurrency.CurrentValue} to device as new target charging currency (currtime)");
                }
                catch (Exception ex)
                {
                    ConsoleHelpers.PrintErrorMessage($"Failed to write {DataPoints.AdjustedCharingCurrency.CurrentValue} to device as new target charging currency (currtime). {ex.Message}");
                }
                Thread.Sleep(2000);
            }
        }

        internal string GetDeviceInformation()
        {
            return ExecuteUDPCommand("i");
        }

        internal string GetDeviceReport1()
        {
            return ExecuteUDPCommand("report 1");
        }
        
        internal string GetDeviceReport2()
        {
            return ExecuteUDPCommand("report 2");
        }
           
        /// <summary>
        /// Writes the charging current to the device
        /// </summary>
        /// <param name="current">Target current in mA, possible values 0; 6000 - 63000</param>
        /// <returns></returns>
        internal void WriteChargingCurrentToDevice(int current)
        {
            ConsoleHelpers.PrintMessage("Updating charging currency to " + current);
            var success = ExecuteUDPCommand($"currtime {current} 1");
            ConsoleHelpers.PrintMessage("Result: " + success);
            if (success != "TCH-OK :done\n")
            {
                ConsoleHelpers.PrintErrorMessage($"Setting currency failed - {success}");
                throw new Exception($"Setting currency failed");
            }
            else
            {
                ConsoleHelpers.PrintMessage("Updated charging currency to " + current);
            }
        }

        internal KebaDeviceStatusData GetDeviceStatus()
        {
            var dataString = "";
            var data = new KebaDeviceStatusData();
            try
            {
                var report2 = ExecuteUDPCommand("report 2");
                var report3 = ExecuteUDPCommand("report 3");
                dataString = report2 + report3;
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
                ConsoleHelpers.PrintErrorMessage($"Error while reading device status: {ex.Message}\n {dataString}");
            }
            return data;
        }

        internal string ExecuteUDPCommand(string command)
        {
            string result = "";

            //Console.WriteLine($"Before Lock {command}");
            //lock (lockObject)
            {
                //Console.WriteLine($"After Lock {command}");
                using (UdpClient udpClient = new UdpClient(uDPPort))
                {
                    //Console.WriteLine($"Inside Using {command}");

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

                        result = returnData.ToString();
                        //Thread.Sleep(500);
                    }
                    catch (Exception e)
                    {
                        ConsoleHelpers.PrintErrorMessage("Error while communicating via UDP with Keba device: " + e.Message);
                    }
                    finally
                    {
                        udpClient.Close();
                    }
                }
            }
            return result;
        }

        public void Dispose()
        {
            //udpClient.Close();
            //udpClient.Dispose();
        }
    }
}