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

    public partial class HandshakeResponse
    {
        [JsonProperty("error_code")]
        public long ErrorCode { get; set; }

        [JsonProperty("result")]
        public HandshakeResponseResult Result { get; set; }
    }

    public partial class HandshakeResponseResult
    {
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
