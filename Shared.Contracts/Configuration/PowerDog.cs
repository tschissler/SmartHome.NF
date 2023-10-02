using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedContracts.Configuration
{
    public class PowerDog
    { 
        public static string Password { get; set; }

        static PowerDog()
        {
            if (Environment.GetEnvironmentVariable("PowerDogPassword") is string password)
            {
                Password = password;
            }
            else
            {
                throw new Exception("EnvironmentVariable PowerDogPassword not set, access to PowerDog will not be possible.");
            }
        }
    }
}
