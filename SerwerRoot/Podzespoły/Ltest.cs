using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.System.Threading;

namespace Serwer.Podzespoły
{
    class Ltest
    {
        private GpioPin pin;
        public Ltest()
        {

            pin = GpioController.GetDefault().OpenPin(3);
            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.High);
            pin.Write(GpioPinValue.Low);
            pin.Write(GpioPinValue.High);
            pin.Write(GpioPinValue.Low);
            pin.Write(GpioPinValue.High);

        }

    }
}
