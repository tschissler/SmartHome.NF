using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace Keba
{

    public class DeviceStatusData
    {
        /// <summary>
        /// Device status
        /// 0 = Startup
        /// 1 = Not ready for charging
        ///     Charging station is not connected to a vehicle, is locked by the authorization function or another mechanism.
        /// 2 = Ready for charging and waiting for reaction from vehicle
        /// 3 = Charging
        /// 4 = Error is present
        /// 5 = Charging process temporarily interrupted because temperature is too high or any other voter denies.
        /// </summary>
        public int State { get; set; }

        public int Error1 { get; set; }
        public int Error2 { get; set; }

        /// <summary>
        /// Plug status of the cable
        /// 0 = No cable is plugged.
        /// 1 = Cable is plugged into charging station.
        /// 3 = Cable is plugged into charging station and locked.
        ///     This is the default idle state for all devices with permanently attached cable.
        /// 5 = Cable is plugged into charging station and vehicle but not locked.
        /// 7 = Cable is plugged into charging station and vehicle, furthermore the cable is locked.
        ///     Charging is not possible until plug state “7” is reached.
        /// </summary>
        [JsonProperty("Plug")]
        public int PlugStatus { get; set; }

        /// <summary>
        /// Authorization is activated (e.g. RFID card)
        /// </summary>
        [JsonProperty("AuthON")]
        public int AuthorizationFunctionActivated { get; set; }

        /// <summary>
        /// Defines if authorization is required
        /// </summary>
        [JsonProperty("Authreq")]
        public int AuthorizationRequired { get; set; }

        /// <summary>
        /// Defines if charging can be enabled (1) or not (0)
        /// </summary>
        [JsonProperty("Enable sys")]
        public int ChargingEnabled { get; set; }

        /// <summary>
        /// Defines if the device is enabled (1) or disabled (0) e.g. via ena 0
        /// </summary>
        [JsonProperty("Enable user")]
        public int DeviceEnabled { get; set; }

        /// <summary>
        /// Current value in mA offered to the vehicle via control pilot signalization. (Signal type: PWM)
        /// </summary>
        [JsonProperty("Max curr")]
        public int MaxCurrency { get; set; }

        /// <summary>
        /// Duty cycle of the control pilot signal. The unit displayed is not % but 0.1%, which means that the value “1000” stands for a duty cycle of 100%. 
        /// For more information concerning the control pilot refer to IEC 61851-1.
        /// </summary>
        [JsonProperty("Max curr %")]
        public int MaxcurrPercent { get; set; }

        /// <summary>
        /// Maximum current value in mA that can be supported by the hardware of the device.
        /// This value represents the minimum of the DIP switch settings, cable coding and temperature monitoring function.
        /// </summary>
        [JsonProperty("Curr HW")]
        public int CurrencySupportedByDevice { get; set; }

        /// <summary>
        /// Current setting in mA defined via UDP current commands. (Default: 63000 mA)
        /// </summary>
        [JsonProperty("Curr user")]
        public int DefinedCurrency { get; set; }

        /// <summary>
        /// Current setting in mA defined via fail-safe function.
        /// </summary>
        [JsonProperty("Curr FS")]
        public int FailSaveCurrency { get; set; }

        public int TmoFS { get; set; }
        public int Currtimer { get; set; }
        public int TmoCT { get; set; }
        public int Setenergy { get; set; }
        public int Output { get; set; }
        public int Input { get; set; }
        public string Serial { get; set; }
        public int Sec { get; set; }
        public int U1 { get; set; }
        public int U2 { get; set; }
        public int U3 { get; set; }
        public int I1 { get; set; }
        public int I2 { get; set; }
        public int I3 { get; set; }
        public int P { get; set; }
        public int PF { get; set; }
        public int Epres { get; set; }
        public int Etotal { get; set; }
    }

}