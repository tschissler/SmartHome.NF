using Keba;
using Smarthome.Web.Components;
using static Smarthome.Web.Components.PowerDogDeviceConnector;

namespace Smarthome.Web.Controllers
{
    public class ChargingController
    {
        private readonly PowerDogDeviceConnector powerDog;
        private readonly KebaDeviceConnector keba;

        public delegate void CalculatedCharingPowerChangedEventHandler(object sender, EventArgs e);
        public event CalculatedCharingPowerChangedEventHandler CalculatedCharingPowerChange;

        DateTime latestUpdateTimeStamp = DateTime.MinValue;


        public ChargingController(PowerDogDeviceConnector powerDog, KebaDeviceConnector keba) 
        {
            this.powerDog = powerDog;
            this.keba = keba;

            powerDog.PVProductionChanged += RecalculateChargingPower;
            powerDog.GridConsumptionChanged += RecalculateChargingPower;
            powerDog.GridSupplyChanged += RecalculateChargingPower;
            keba.KebaDeviceStatusChanged += RecalculateChargingPower;
        }

        private void RecalculateChargingPower(object sender, EventArgs e)
        {
            if (DateTime.Now.Subtract(latestUpdateTimeStamp).TotalMilliseconds > 1000)
            {
                latestUpdateTimeStamp = DateTime.Now;
                // Lieferung + Auto - Bezug
                double calculatedChargingPower = powerDog.GridSupply + keba.CurrentChargingPower - powerDog.GridConsumption;
                double calculatedChargingCurrent = calculatedChargingPower / 230 * 1000;
                if (calculatedChargingCurrent >= 6000)
                {
                    keba.SetCurrent((int)calculatedChargingCurrent);
                }
                CalculatedCharingPowerChange?.Invoke(this, new DataChangedEventArgs(calculatedChargingPower));
            }
        }
    }
}
