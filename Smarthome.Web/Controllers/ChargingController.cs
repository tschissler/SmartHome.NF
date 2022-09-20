using Keba;
using Smarthome.Web.Components;
using static Smarthome.Web.Components.PowerDogDeviceConnector;

namespace Smarthome.Web.Controllers
{
    public class ChargingController
    {
        private readonly PowerDogDeviceConnector powerDog;
        private readonly KebaDeviceConnector keba;
        private DateTime latestUpdateTimeStamp = DateTime.MinValue;
        private bool autoCharging;
        private int manualChargingCurrency;
        private int previousChargingCurrency;

        public delegate void CalculatedCharingPowerChangedEventHandler(object sender, EventArgs e);
        public event CalculatedCharingPowerChangedEventHandler CalculatedCharingPowerChange;

        public bool MinCharging { get; set; }

        public int ManualChargingCurrency
        {
            get
            {
                return manualChargingCurrency;
            }
            set
            {
                manualChargingCurrency = value;
                if (manualChargingCurrency < 6000)
                {
                    manualChargingCurrency = 0;
                }
                if (!autoCharging)
                {
                    keba.SetChargingCurrent(manualChargingCurrency);
                }
            }
        }
        public bool AutoCharging
        {
            get
            {
                return autoCharging;
            }
            set
            {
                if (autoCharging != value)
                {
                    autoCharging = value;
                    if (!autoCharging)
                    {
                        keba.SetChargingCurrent(ManualChargingCurrency);
                        powerDog.PVProductionChanged -= RecalculateChargingPower;
                        powerDog.GridConsumptionChanged -= RecalculateChargingPower;
                        powerDog.GridSupplyChanged -= RecalculateChargingPower;
                        keba.KebaDeviceStatusChanged -= RecalculateChargingPower;
                    }
                    else
                    {
                        powerDog.PVProductionChanged += RecalculateChargingPower;
                        powerDog.GridConsumptionChanged += RecalculateChargingPower;
                        powerDog.GridSupplyChanged += RecalculateChargingPower;
                        keba.KebaDeviceStatusChanged += RecalculateChargingPower;
                    }
                }
            }
        }

        public ChargingController(PowerDogDeviceConnector powerDog, KebaDeviceConnector keba)
        {
            this.powerDog = powerDog;
            this.keba = keba;

            //powerDog.PVProductionChanged += RecalculateChargingPower;
            //powerDog.GridConsumptionChanged += RecalculateChargingPower;
            //powerDog.GridSupplyChanged += RecalculateChargingPower;
            //keba.KebaDeviceStatusChanged += RecalculateChargingPower;
        }

        private void RecalculateChargingPower(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(latestUpdateTimeStamp).TotalMilliseconds > 1000)
            {
                latestUpdateTimeStamp = DateTime.Now;
                // Lieferung + Auto - Bezug
                double calculatedChargingPower = powerDog.GridSupply + keba.CurrentChargingPower / 1000 - powerDog.GridConsumption;
                double calculatedChargingCurrent = calculatedChargingPower / 230 * 1000 / 3;
                if (calculatedChargingCurrent < 6000)
                {
                    calculatedChargingCurrent = MinCharging ? 6000 : 0;
                }

                if ((int)calculatedChargingCurrent != previousChargingCurrency)
                {
                    keba.SetChargingCurrent((int)calculatedChargingCurrent);
                    previousChargingCurrency = (int)calculatedChargingCurrent;
                }
                CalculatedCharingPowerChange?.Invoke(this, new DataChangedEventArgs(calculatedChargingPower));

                Console.WriteLine($"Grid Supply: {powerDog.GridSupply} \t| Charging: {(keba.CurrentChargingPower/1000).ToString("0.0")} \t| Consumption: {powerDog.GridConsumption} \t| Calculated Power: {calculatedChargingPower.ToString("0.0")} \t | Calculated Currency: {calculatedChargingCurrent.ToString("0.0")} \t| Previous: {previousChargingCurrency.ToString("0.0")}");
            }
        }
    }
}
