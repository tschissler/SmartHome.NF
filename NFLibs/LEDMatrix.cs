using Iot.Device.Max7219;
using nanoFramework.Hardware.Esp32;
using System.Device.Spi;

namespace NFLibs
{
    public class LEDMatrix
    {
        private Max7219 devices;
        private MatrixGraphics graphics;
        private IFont currentFont = Fonts.LCDMonoSpace;
        SpiDevice spi;

        public LEDMatrix(int cascadedDevices)
        {
            Configuration.SetPinFunction(23, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(19, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(18, DeviceFunction.SPI1_CLOCK);

            int chipSelect = 5;

            SpiConnectionSettings connectionSettings = new(1, chipSelect)
            {
                ClockFrequency = Iot.Device.Max7219.Max7219.SpiClockFrequency,
                Mode = Iot.Device.Max7219.Max7219.SpiMode
            };
            spi = SpiDevice.Create(connectionSettings);
            devices = new(spi, cascadedDevices: cascadedDevices, rotation: RotationType.Right, rotateFullDisplay: true);
            // initialize the devices
            devices.Init();

            // reinitialize the devices
            devices.Init();

            graphics = new(devices, Fonts.Default);
        }

        public void SetBigthness(int brightness)
        {
            devices.Brightness(brightness);
        }

        public void SetFont(IFont font)
        {
            currentFont = font;
        }

        public void ShowText(string text, int brightness, int characterSpace = 1)
        {
            devices.Brightness(brightness);
            //devices.Init();
            var g = new MatrixGraphics(devices, currentFont);
            g.ShowMessage(text, alwaysScroll: false, characterSpace: characterSpace);
        }
    }
}
