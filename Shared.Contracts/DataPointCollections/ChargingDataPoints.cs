using SharedContracts.DataPoints;
using System.Runtime.Serialization;

namespace SharedContracts.DataPointCollections
{
    public class ChargingDataPoints
    {
        // UI
        // --------------------------------------------------------------------------------------
        /// <summary>
        /// Defines whether charging is controlled by PV production or set to a manual value
        /// </summary>
        public BooleanDataPoint AutomaticCharging = new() { CurrentValue = true };
        /// <summary>
        /// Defines the minimum share of PV production that should be used for charging.
        /// </summary>
        /// <remarks>
        /// If the PV production is lower than this value, charging will be stopped.
        /// </remarks>
        public DecimalDataPoint MinimumPVShare = new() { Label = "Mindestanteil PV-Strom", MaxValue = 100, Unit = "%", CurrentValue = 100 };
        /// <summary>
        /// Defines the manual charging currency that is used if automatic charging is disabled
        /// </summary>
        /// <seealso cref="AutomaticCharging"/>
        public DecimalDataPoint ManualChargingCurrency = new() { Label = "Manueller Lade-Strom", MaxValue = 16, Unit = "A", DecimalDigits = 1 };

        // Keba
        // --------------------------------------------------------------------------------------
        /// <summary>
        /// The current charging power the car consumes in W
        /// </summary>
        public DecimalDataPoint CurrentChargingPower = new() { Label = "Aktuelle Lade-Leistung", Unit = "W", MaxValue = 12000, DecimalDigits = 1 };
        /// <summary>
        /// The maximum charging currency set on the device in A
        /// </summary>
        /// <remarks>
        /// This is either defined by setting it via currtime UDP command or the devices hardware configuration depending on which one is lower
        /// </remarks>
        public DecimalDataPoint EffectiveMaximumChargingCurrency = new() { Label = "Aktuell eingestellter Strom", Unit = "A", MaxValue = 16, DecimalDigits = 1 };
        /// <summary>
        /// The voltage at the charging point as an average value over all 3 phases in V
        /// </summary>
        /// <remarks>
        /// The voltage is 0 if no charging is in progress
        /// </remarks>
        public DecimalDataPoint CurrentVoltage = new() { Label = "Aktuelle Spannung", Unit = "V", MaxValue = 250, DecimalDigits = 1 };
        /// <summary>
        /// The consumption during the cerrent / latest charging session in Wh
        /// </summary>
        public DecimalDataPoint ConsumptionActiveSession = new() { Label = "Ladung Sitzung", Unit = "Wh", MaxValue = 80000 };
        /// <summary>
        /// The overall consumption over all sessions in KWh
        /// </summary>
        public DecimalDataPoint CharingOverallTotal = new() { Label = "Ladung gesamt", Unit = "KWh", DecimalDigits = 1 };
        //public DecimalDataPoint CarLatestChargingPower = new() { Label = "Ladeleistung", Unit = "W", MaxValue = 12000, DecimalDigits = 1 };
        //public DecimalDataPoint CarChargingCurrentTarget = new() { Label = "Strom Verfügbar", Unit = "A", MaxValue = 16, DecimalDigits = 1 };
        //public IntegerDataPoint CarChargingManualCurrency = new() { Unit = "mA", MaxValue = 16000 };

        /// <summary>
        /// The current status of the charging station
        /// </summary>
        /// <remarks>
        /// { 0, "Warten" }, { 1, "Nicht bereit" }, { 2, "Bereit" }, { 3, "Laden" }, { 4, "Fehler" }, { 5, "Unterbrochen" }
        /// </remarks>
        public IntegerDataPoint KebaStatus = new();
        //public IntegerDataPoint TargetChargingCurrent = new() { Label = "Zielwert für Ladestrom", Unit = "mA", MaxValue = 16000 };


        // Calculation
        // --------------------------------------------------------------------------------------
        /// <summary>
        /// The current electricity supply to the grid
        /// </summary>
        public DecimalDataPoint GridSupply = new() { Label = "Einspeisung", Unit = "W", MaxValue = 12000, DecimalDigits = 1, History = new() { DataHistoryLength = 100 } };
        /// <summary>
        /// The current demand of electricity from the grid
        /// </summary>
        public DecimalDataPoint GridDemand = new() { Label = "Bezug", Unit = "W", MaxValue = 8000, DecimalDigits = 1, History = new() { DataHistoryLength = 100 } };
        /// <summary>
        /// Saldo of electricity (Supply - Demand)
        /// </summary>
        public DecimalDataPoint GridSaldo = new() { Label = "Netz Saldo", Unit = "W", DecimalDigits = 1 };
        /// <summary>
        /// The calculated power available for charging
        /// </summary>
        public DecimalDataPoint AvailableChargingPower = new() { Label = "Ladeleistung Berechnet", Unit = "W", DecimalDigits = 1 };
        /// <summary>
        /// The calculated currency available for charging based on CalculatedAvailableChargingPower in A
        /// </summary>
        public DecimalDataPoint AvailableChargingCurrency = new() { Label = "Ladestrom Berechnet", Unit = "A", DecimalDigits = 1 };

        //public DecimalDataPoint MinimumChargingCurrency = new() { Label = "Mindestladestrom", Unit = "A", DecimalDigits = 1, CurrentValue = 6 };
        /// <summary>
        /// The currency that must be available to start charging as defined by the MinimumPVShare
        /// </summary>
        /// <seealso cref="MinimumPVShare"/>
        public DecimalDataPoint MinimumActivationPVCurrency = new() { Label = "Aktivierung ab PV-Strom", Unit = "A", DecimalDigits = 1 };

        // Configuration
        // --------------------------------------------------------------------------------------
        /// <summary>
        /// The charging currency that will be used to configure the charging station
        /// </summary>
        public DecimalDataPoint AdjustedCharingCurrency = new() { Label = "Effektiv eingestellter Ladestrom", Unit = "A", DecimalDigits = 1 };

    }
}
