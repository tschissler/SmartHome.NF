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

    public partial class HandshakeRequest
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public HandshakeRequestParams Params { get; set; }

        [JsonProperty("requestTimeMils")]
        public long RequestTimeMils { get; set; }
    }

    public partial class HandshakeRequestParams
    {
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}
