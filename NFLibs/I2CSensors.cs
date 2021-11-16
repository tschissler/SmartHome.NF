using Iot.Device.Bh1750fvi;
using Iot.Device.Bmp180;
using Iot.Device.Shtc3;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.I2c;
using UnitsNet;

namespace NFLibs
{
    public class I2CSensors
    {
        const int busId = 1;

        private static bool isInitialized = false;
        private static I2cDevice i2cDevice;
        private static Bmp180 i2cBmp180;
        private static Bh1750fvi bh1750sensor;
        private static Shtc3 shtc3Sensor;

        public static bool Init(int dataPin, int clockPin, bool useBMP180, bool useBH1750, bool useSHTC3)
        {
            Configuration.SetPinFunction(dataPin, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(clockPin, DeviceFunction.I2C1_CLOCK);

            if (useBMP180)
            {
                I2cConnectionSettings i2cSettings = new(busId, Bmp180.DefaultI2cAddress);
                i2cDevice = I2cDevice.Create(i2cSettings);
                i2cBmp180 = new(i2cDevice);
                // set samplings
                i2cBmp180.SetSampling(Sampling.Standard);
            }

            if (useBH1750)
            {
                I2cConnectionSettings settings = new I2cConnectionSettings(busId: 1, (int)I2cAddress.AddPinLow);
                I2cDevice device = I2cDevice.Create(settings);

                bh1750sensor = new Bh1750fvi(device);
            }

            if (useSHTC3)
            {
                I2cConnectionSettings settings = new I2cConnectionSettings(1, Shtc3.DefaultI2cAddress);
                I2cDevice device = I2cDevice.Create(settings);

                shtc3Sensor = new Shtc3(device);
            }

            isInitialized = true;
            return true;
        }

        public static double ReadBMP180Temperature()
        {
            if (!isInitialized)
                throw new Exception("Call Init-method before reading data");
            Temperature tempValue = i2cBmp180.ReadTemperature();
            return tempValue.DegreesCelsius;
        }

        public static double ReadBH1750Illuminance()
        {
            if (!isInitialized)
                throw new Exception("Call Init-method before reading data");
            return bh1750sensor.Illuminance.Lux;
        }
        public static double ReadSHTC3Temperature()
        {
            if (!isInitialized)
                throw new Exception("Call Init-method before reading data");
            if (shtc3Sensor.TryGetTemperatureAndHumidity(out var temperature, out var relativeHumidity))
            {
                return temperature.DegreesCelsius;
            }
            else
            {
                return -999;
            }
        }

        public static double ReadSHTC3Humitidy()
        {
            if (!isInitialized)
                throw new Exception("Call Init-method before reading data");
            if (shtc3Sensor.TryGetTemperatureAndHumidity(out var temperature, out var relativeHumidity))
            {
                return relativeHumidity.Percent;
            }
            else
            {
                return -999;
            }
        }
    }
}
