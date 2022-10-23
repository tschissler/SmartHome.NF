using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapoLib
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class LoginDeviceResponseDecrypted
    {
        [JsonProperty("error_code")]
        public long ErrorCode { get; set; }

        [JsonProperty("result")]
        public LoginDeviceResponseDecryptedResult Result { get; set; }
    }

    public partial class LoginDeviceResponseDecryptedResult
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}