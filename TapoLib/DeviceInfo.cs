using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapoLib
{
    public class DeviceInfo
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public string SessionId { get; set; }
        public string Token { get; set; }
    }
}
