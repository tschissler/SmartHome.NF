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

    public partial class LoginDeviceResponse
    {
        [JsonProperty("error_code")]
        public long ErrorCode { get; set; }

        [JsonProperty("result")]
        public LoginDeviceResponseResult Result { get; set; }
    }

    public partial class LoginDeviceResponseResult
    {
        [JsonProperty("response")]
        public string Response { get; set; }
    }
}