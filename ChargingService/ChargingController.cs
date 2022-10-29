using HelpersLib;
using KebaLib;
using Newtonsoft.Json;
using PowerDogLib;
using Secrets;
using SharedContracts;
using SharedContracts.DataPointCollections;
using SharedContracts.DataPoints;
using SharedContracts.StorageData;
using StorageLib;
using System.Net;

namespace ChargingService
{
    public class ChargingController
    {
        private PowerDog powerDog;
        private KebaDeviceConnector kebaConnector;
        private double lastSetChargingCurrency;
        private Timer recalculationTimer;
        private TimeSpan recalculationFrequency = TimeSpan.FromSeconds(2);
        private Timer storageTimer;
        private TimeSpan storageFrequency = TimeSpan.FromSeconds(2);
        private DateTime chargingSessionStartTime;
        private DateTime chargingSessionLastChargingTime;
        private int previousChargingState;
        public object lockobject = new object();

        public ChargingController()
        {
            Dictionary<string, string> sensorKeys = new()
            {
                { "Bezug", "iec1107_1457430339" }, // Vom Zähler
                { "Erzeugung", "pv_global_1454412642" },  // Vom Wechselrichter
                { "Eigenverbrauch", "arithmetic_1457431399" },
                { "Verbrauchgesamt", "arithmetic_1457432629" },
                { "lieferung", "iec1107_1457430562" } // Vom Zähler
            };
            powerDog = new PowerDog(sensorKeys, new UriBuilder("http", "192.168.178.150", 20000).Uri, PowerDogSecrets.Password);
            kebaConnector = new KebaDeviceConnector(new IPAddress(new byte[] { 192, 168, 178, 167 }), 7090);

            recalculationTimer = new Timer(CalculateData, null, 0, (int)recalculationFrequency.TotalMilliseconds);
            //if (Environment.GetEnvironmentVariable("WRITE_TABLE_TO_TABLESTORAGE") != null && Environment.GetEnvironmentVariable("WRITE_TABLE_TO_TABLESTORAGE").ToLower() == "false")
            //{
            //    ConsoleHelpers.PrintInformation("WRITE_TABLE_TO_TABLESTORAGE is set to false, so we will not write data to the cloud");
            //}
            //else
            //{
            //    storageTimer = new Timer(StoreData, null, (int)storageFrequency.TotalMilliseconds, (int)storageFrequency.TotalMilliseconds);
            //}
        }

        public ChargingDataPoints GetDataPoints()
        {
            lock (lockobject)
            {
                return kebaConnector.DataPoints;
            }
        }

        public void SetChargingCurrency(double current)
        {
            kebaConnector.SetChargingCurrent(current);
        }

        public void ApplyChargingSettings(ChargingSettingsData chargingSettingsData)
        {
            kebaConnector.DataPoints.AutomaticCharging.CurrentValue = chargingSettingsData.automaticCharging;
            kebaConnector.DataPoints.MinimumPVShare.CurrentValue = chargingSettingsData.minimumPVShare;
            kebaConnector.DataPoints.ManualChargingCurrency.CurrentValue = chargingSettingsData.manualCurrency;
        }
        
        public ChargingSettingsData GetChargingSettings()
        {
            ChargingSettingsData chargingSettingsData = new()
            {
                automaticCharging = kebaConnector.DataPoints.AutomaticCharging.CurrentValue,
                minimumPVShare = kebaConnector.DataPoints.MinimumPVShare.CurrentValue,
                manualCurrency = kebaConnector.DataPoints.ManualChargingCurrency.CurrentValue
            };
            return chargingSettingsData;
        }

        public void StoreData(object? state)
        {
            lock (lockobject)
            {
                try
                {
                    var timeStamp = DateTime.UtcNow;
                    EnergyM3StorageData storageEntity = new()
                    {
                        RowKey = timeStamp.ToString("yyyyMMddHHmmss"),
                        PartitionKey = timeStamp.ToString("yyyyMMddHHmm"),
                        Timestamp = timeStamp,

                        GridDemand = powerDog.DataPoints.GridDemand.CurrentValue,
                        GridSupply = powerDog.DataPoints.GridSupply.CurrentValue,
                        PVProduction = powerDog.DataPoints.PVProduction.CurrentValue,
                        CarChargingStatus = kebaConnector.DataPoints.KebaStatus.CurrentValue,
                        CarCharging = kebaConnector.DataPoints.CurrentChargingPower.CurrentValue
                    };
                    TableStorageConnector.WriteEnergyM3DataToTable(storageEntity);
                }
                catch (Exception ex)
                {
                    ConsoleHelpers.PrintErrorMessage($"Error while storing data: {ex.Message}");
                }
            }
        }
        
        public void CalculateData(object? state)
        {
            if (kebaConnector.DataPoints.KebaStatus.CurrentValue == 1)
            {
                if (previousChargingState != 1)
                {
                    Console.WriteLine("Car not connected, no charging calculated.");
                    previousChargingState = kebaConnector.DataPoints.KebaStatus.CurrentValue;
                }
                return;
            }
            //lock (lockobject)
            if (Monitor.TryEnter(lockobject, 1000))
            {
                try
                {
                    // The minimum currency the charging station needs to start charging, which is 6A
                    var minimumChargingCurrency = 6;
                    var chargingStartOffset = 0.1;
                    var previousChargingCurrency = kebaConnector.DataPoints.AvailableChargingCurrency.CurrentValue;

                    powerDog.ReadSensorsData(state);

                    // Calculation
                    ChargingDataPoints dataPoints = kebaConnector.DataPoints;
                    dataPoints.GridSupply.CurrentValue = powerDog.DataPoints.GridSupply.CurrentValue;
                    dataPoints.GridDemand.CurrentValue = powerDog.DataPoints.GridDemand.CurrentValue;
                    dataPoints.GridSaldo.CurrentValue = powerDog.DataPoints.GridSupply.CurrentValue - powerDog.DataPoints.GridDemand.CurrentValue;
                    dataPoints.AvailableChargingPower.CurrentValue = dataPoints.GridSaldo.CurrentValue + dataPoints.CurrentChargingPower.CurrentValue;
                    dataPoints.AvailableChargingCurrency.CurrentValue = dataPoints.CurrentVoltage.CurrentValue > 0 ? dataPoints.AvailableChargingPower.CurrentValue / dataPoints.CurrentVoltage.CurrentValue / 3 : dataPoints.AvailableChargingPower.CurrentValue / 230 / 3;
                    // Limit increasing of charging to 10% per cycle
                    if (previousChargingCurrency > minimumChargingCurrency && dataPoints.AvailableChargingCurrency.CurrentValue > previousChargingCurrency * 1.1)
                    {
                        dataPoints.AvailableChargingCurrency.CurrentValue = previousChargingCurrency * 1.1;
                    }
                    dataPoints.MinimumActivationPVCurrency.CurrentValue = minimumChargingCurrency * dataPoints.MinimumPVShare.CurrentValue / 100;

                    if (dataPoints.KebaStatus.CurrentValue == 3 && previousChargingState != 3)
                    {
                        chargingSessionStartTime = DateTime.Now;
                    }
                    previousChargingState = dataPoints.KebaStatus.CurrentValue;

                    if (dataPoints.AvailableChargingCurrency.CurrentValue >= minimumChargingCurrency)
                    {
                        ConsoleHelpers.PrintMessage($"----> Available charging currency ({dataPoints.AvailableChargingCurrency.AssembleValueString()}) is high enough to start charging");
                        dataPoints.AdjustedCharingCurrency.CurrentValue = dataPoints.AvailableChargingCurrency.CurrentValue;
                        chargingSessionLastChargingTime = DateTime.Now;
                    }
                    else if (!dataPoints.AutomaticCharging.CurrentValue && dataPoints.ManualChargingCurrency.CurrentValue >= 6)
                    {
                        ConsoleHelpers.PrintMessage($"----> Automatic charging is off and Manual charging currency ({dataPoints.ManualChargingCurrency.AssembleValueString()}) is greater than 0 so start charging");
                        dataPoints.AdjustedCharingCurrency.CurrentValue = dataPoints.ManualChargingCurrency.CurrentValue;
                        chargingSessionLastChargingTime = DateTime.Now;
                    }
                    else if (dataPoints.AutomaticCharging.CurrentValue && dataPoints.AvailableChargingCurrency.CurrentValue >= dataPoints.MinimumActivationPVCurrency.CurrentValue)
                    {
                        ConsoleHelpers.PrintMessage($"----> Automatic charging is on and Available charging currency ({dataPoints.AvailableChargingCurrency.AssembleValueString()}) is greater than Minimum activation PV currency ({dataPoints.MinimumActivationPVCurrency.CurrentValue}) so start charging");
                        dataPoints.AdjustedCharingCurrency.CurrentValue = minimumChargingCurrency;
                        chargingSessionLastChargingTime = DateTime.Now;
                    }
                    else
                    {
                        if (dataPoints.AutomaticCharging.CurrentValue)
                        {
                            // Problem mit Wechselrichter, schaltet ab , wenn Ladesitzung startet.
                            // Damit Ladesitzung dann nicht sofort beendet wird, wird eine Mindestdauer von 60 Sek. erzwungen.
                            if ((DateTime.Now - chargingSessionStartTime).TotalSeconds < 60)
                            {
                                ConsoleHelpers.PrintMessage($"----> Available charging currency too low but charging session shorter than 60sec, so continuing");
                                dataPoints.AdjustedCharingCurrency.CurrentValue = minimumChargingCurrency;
                            }
                            else
                            {
                                // Min. 10 Sek. weiterladen um kurfristige Aussetzer zu kompensieren
                                if ((DateTime.Now - chargingSessionLastChargingTime).TotalSeconds < 10)
                                {
                                    ConsoleHelpers.PrintMessage($"----> Available charging currency too low but waiting for 10 more seconds before stopping but reducing to min currency yet.");
                                    dataPoints.AdjustedCharingCurrency.CurrentValue = minimumChargingCurrency;
                                }
                                else
                                {
                                    ConsoleHelpers.PrintMessage($"----> Available charging currency ({dataPoints.AvailableChargingCurrency.AssembleValueString()}) is not high enough to start charging");
                                    dataPoints.AdjustedCharingCurrency.CurrentValue = 0;
                                }
                            }
                        }
                        else
                        {
                            ConsoleHelpers.PrintMessage($"----> Automatic charging is off and Manual charging currency ({dataPoints.ManualChargingCurrency.AssembleValueString()}) is not greater than 0 so don't start charging");
                            dataPoints.AdjustedCharingCurrency.CurrentValue = 0;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(lockobject);
                }
            }
        }
    }
}
