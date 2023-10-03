using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.Configuration
{
    public class Keba
    {
        public static string KebaIP_CarPort{ get; set; }

        static Keba()
        {
            foreach (var field in typeof(Keba).GetProperties())
            {
                if (Environment.GetEnvironmentVariable(field.Name) is string envVariableValue)
                {
                    field.SetValue(null, envVariableValue);
                }
                else
                {
                    throw new Exception($"---> EnvironmentVariable {field.Name} is not set, execution will stop as a mandatory configuration is missing.");
                }
            }
        }
    }
}
