using HelpersLib;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Shared.Contracts;
using Newtonsoft.Json.Converters;

namespace HomematicLib
{
    public class HomematicConnector
    {
        public static async Task<HomeMaticData> ReadData(Uri deviceAddress)
        {
            string body = "";
            HomeMaticData value = new HomeMaticData();
            try
            {
                body = await executePost(new Uri("https://lookup.homematic.com:48335/getHost  "));
                string url = JsonConvert.DeserializeObject<GetHostResponse>(body).urlREST + "/hmip/home/getCurrentState";
                body = await executePost(new Uri(url));
                value = JsonConvert.DeserializeObject<HomeMaticData>(body);
            }
            catch (Exception ex)
            {
                ConsoleHelpers.PrintErrorMessage($"Failed to read data from Homematic cloud {deviceAddress}, Error: " + ex.Message);
            }
            return value;
        }

        private static async Task<string> executePost(Uri url)
        {
            string body = "";
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = url,
                Headers =
                    {
                        { "accept", "application/json" },
                        { "VERSION", "12" },
                        { "AUTHTOKEN", "F65D918ADF494A9A3A9212600C77B4DA5E155A4D6B25E29E3F7C22E26DF01C2D" },
                        { "CLIENTAUTH", "EB00EE2ABD46369B72BF92B9A4E90A1AB433753858A4D8F4F72B27364093BD58067113DC30E814F49DB8902AC86C96675486EC5A0E1328552CC2F8B4290FD1B6" },
                    },
                Content = new StringContent("{\n  \"clientCharacteristics\": \n\t{\n\t\t\"apiVersion\": \"10\", \n\t\t\"applicationIdentifier\": \"homematicip-python\",\n\t\t\"applicationVersion\": \"1.0\", \n\t\t\"deviceManufacturer\": \"none\", \n\t\t\"deviceType\": \"Computer\", \n\t\t\"language\": \"en_US\", \n\t\t\"osType\": \"Linux\", \n\t\t\"osVersion\": \"5.10.102.1-microsoft-standard-WSL2\"\n\t}, \n\t\"id\": \"3014F711A00003DD89B51BC4\"\n}")
                {
                    Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                body = await response.Content.ReadAsStringAsync();

                //DateTimeOffset.FromUnixTimeMilliseconds(1668118285862)
            }

            return body;
        }
    }


    public class GetHostResponse
    {
        public string urlREST { get; set; }
        public string urlWebSocket { get; set; }
        public string apiVersion { get; set; }
        public string primaryAccessPointId { get; set; }
        public string requestingAccessPointId { get; set; }
    }


    public class HomeMaticData
    {
        public Devices devices { get; set; }
    }
    public class Devices
    {
        [JsonProperty(PropertyName = "3014F711A00000DD89B23DF8")]
        public DeviceData WindowContactKidsRoom { get; set; }
        [JsonProperty(PropertyName = "3014F711A00000DD89B23DEC")]
        public DeviceData WindowContactBathRoom { get; set; }
        [JsonProperty(PropertyName = "3014F711A0000A1D89B0DAD1")]
        public DeviceData ThermostatKidsRoom { get; set; }
        [JsonProperty(PropertyName = "3014F711A0000A1D89B0D9F3")]
        public DeviceData ThermostatBathRoom { get; set; }

    }

    public class DeviceData
    {
        public string id { get; set; }
        public string type { get; set; }
        //[JsonConverter(typeof(UnixDateTimeConverter))]
        public long lastStatusUpdate { get; set; }
        public string label { get; set; }
        public Functionalchannels functionalChannels { get; set; }
    }

    public class Functionalchannels
    {
        [JsonProperty(PropertyName = "1")]
        public ChannelData DeviceData { get; set; }
    }

    public class ChannelData
    {
        public string functionalChannelType { get; set; }
        public string channelRole { get; set; }
        public string windowState { get; set; }
        public float setPointTemperature { get; set; }
        public float valveActualTemperature { get; set; }
    }
}