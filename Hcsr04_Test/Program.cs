using System;
using System.Diagnostics;
using System.Threading;
//using UnitsNet;
using System.Collections;
//using Iot.Device.Hcsr04.Esp32;
using Windows.Devices.Adc;

namespace Hcsr04_Test
{
    public class Program
    {
        //private const int DistanceSensor_Trigger_Pin = 14;
        //private const int DistanceSensor_Echo_Pin = 12;
        //private static Hcsr04 DistanceSensor;
        //private static double minDistance = 999;
        //private static double maxDistance;

        public static void Main()
        {
            //DistanceSensor = new Hcsr04(DistanceSensor_Trigger_Pin, DistanceSensor_Echo_Pin);
            //var measurements = new System.Collections.ArrayList();
            //double sum;

            //for (int i = 0; i < 100; i++)
            //{
            //    if (DistanceSensor.TryGetDistance(out Length distance))
            //    {
            //        Debug.WriteLine($"Distance: {distance.Centimeters} cm");
            //        if (distance.Centimeters < minDistance)
            //            minDistance = distance.Centimeters;
            //        if (distance.Centimeters > maxDistance)
            //            maxDistance = distance.Centimeters;
            //        measurements.Add(distance.Centimeters);

            //        sum = 0;
            //        foreach (var measurement in measurements)
            //        {
            //            sum += (double)measurement;
            //        }

            //        Debug.WriteLine($"Min: {minDistance} | Max: {maxDistance} | Var: {maxDistance - minDistance} | Avg: {sum/measurements.Count}");
            //    }
            //    else
            //    {
            //        Debug.WriteLine("Error reading sensor for distance");
            //    }
            //    Thread.Sleep(1000);
            //}

            //Thread.Sleep(Timeout.Infinite);

            string devs = AdcController.GetDeviceSelector();

            Debug.WriteLine("devs=" + devs);

            AdcController adc1 = AdcController.GetDefault();

            int max1 = adc1.MaxValue;
            int min1 = adc1.MinValue;

            Debug.WriteLine("min1=" + min1.ToString() + " max1=" + max1.ToString());

            AdcChannel ac0 = adc1.OpenChannel(4);

            while (true)
            {
                int value = ac0.ReadValue();

                double percent = ac0.ReadRatio();

                Debug.WriteLine("value0=" + value.ToString() + " ratio=" + percent.ToString());

                //Thread.Sleep(1000);
            }
        }
    }
}
