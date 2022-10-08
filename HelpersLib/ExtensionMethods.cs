using SharedContracts.DataPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLib
{
    public static class ExtensionMethods
    {
        public static string AssembleValueString(this DecimalDataPoint dataPoint)
        {
            string format = "0." + new String('0', dataPoint.DecimalDigits);
            return $"{dataPoint.CurrentValue.ToString(format)} {dataPoint.Unit}";
        }
    }
}
