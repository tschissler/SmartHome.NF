using SharedContracts.DataPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.DataPointCollections
{
    public class ChargingControlDataPoints
    {
        public BooleanDataPoint AutomaticCharging = new ();
        public DecimalDataPoint MinimumPVShare = new () { Label = "Mindestanteil PV-Strom", MaxValue=100, Unit="%"};
        public DecimalDataPoint ManualChargingCurrency = new() { Label = "Manueller Lade-Strom", MaxValue = 16, Unit = "A", DecimalDigits=1 };

        public DecimalDataPoint GridSupply = new() { Label = "Einspeisung", Unit = "W", MaxValue = 12000, DecimalDigits = 1 };
        public DecimalDataPoint GridDemand = new() { Label = "Bezug", Unit = "W", MaxValue = 8000, DecimalDigits = 1 };
        public DecimalDataPoint CurrentChargingPower = new() { Label = "Aktuelle Lade-Leistung", Unit = "W", MaxValue = 12000, DecimalDigits = 1 };
        public DecimalDataPoint CurrentChargingDesiredCurrency = new() { Label = "Aktuell eingestellter Strom", Unit = "A", MaxValue = 16, DecimalDigits = 1 };

        public DecimalDataPoint GridSaldo = new() { Label = "Netz Saldo", Unit = "W", DecimalDigits = 1 };
        public DecimalDataPoint CalculatedChargingPower = new() { Label = "Ladeleistung Berechnet", Unit = "W", DecimalDigits = 1 };
        public DecimalDataPoint CalculatedChargingCurrency = new() { Label = "Ladestrom Berechnet", Unit = "A", DecimalDigits = 1 };
    }
}
