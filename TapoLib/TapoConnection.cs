using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;
using System.Drawing;
using System.Net.Http;

namespace TapoLib
{
    public class TapoConnection
    {
        readonly private string _email, _password, _deviceIP;

        private HttpClient _client = new HttpClient();
        private RequestCipher _requestCipher;
        private KeyPair _keyPair;

        public TapoConnection(string email, string password, string deviceIP)
        {
            _email = email;
            _password = password;
            _deviceIP = deviceIP;
        }

        private async Task<string> SecurePasstrough(string request, string cookie, string token = "")
        {
            object passtroughRequest = new BasicRequest
            {
                Method = "securePassthrough",
                Params = new BasicRequestParams
                {
                    Request = _requestCipher.Encrypt(request)
                }
            };

            var response =
                await RequestWithHeader(
                    $"http://{_deviceIP}/app?token={token}",
                    JsonConvert.SerializeObject(passtroughRequest),
                    cookie);

            string decryptedData = _requestCipher.Decrypt(response);

            return decryptedData;
        }

        public async Task<DeviceInfo> LoginWithIP()
        {
            DeviceInfo deviceInfo = await Handshake();

            _requestCipher = new RequestCipher(deviceInfo.Key, deviceInfo.IV);

            object loginDeviceRequest = new LoginDeviceRequest
            {
                Method = "login_device",
                RequestTimeMils = 0,
                Params = new LoginDeviceRequestParams
                {
                    Username = Convert.ToBase64String(Encoding.UTF8.GetBytes(ShaDigest(_email))),
                    Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(_password)),
                }
            };

            string responseJson = await SecurePasstrough(
                JsonConvert.SerializeObject(loginDeviceRequest),
                deviceInfo.SessionId);

            LoginDeviceResponseDecrypted response = JsonConvert.DeserializeObject<LoginDeviceResponseDecrypted>(responseJson);

            deviceInfo.Token = response.Result.Token;

            return deviceInfo;
        }

        public async Task<string> GetDeviceRunningInfo(DeviceInfo deviceInfo)
        {
            string data =
              "{\"method\":  \"get_device_running_info\", " +
              "\"requestTimeMils\":0, " +
              "\"terminalUUID\": \"" + deviceInfo.SessionId + "\" " +
              "}";

            var responseJson = await SecurePasstrough(
                data,
                deviceInfo.SessionId,
                deviceInfo.Token);

            return responseJson;
        }

        public async Task<string> GetEnergyUsage(DeviceInfo deviceInfo)
        {
            string data =
              "{\"method\":  \"get_energy_usage\", " +
              "\"requestTimeMils\":0, " +
              "\"terminalUUID\": \"" + deviceInfo.SessionId + "\" " +
              "}";

            var responseJson = await SecurePasstrough(
                data,
                deviceInfo.SessionId,
                deviceInfo.Token);

            return responseJson;
        }

        private string ShaDigest(string data)
        {
            using var sha1 = SHA1.Create();
            return Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }

        private async Task<DeviceInfo> Handshake()
        {
            _keyPair = new KeyPair();

            object handshake = new HandshakeRequest
            {
                Method = "handshake",
                Params = new HandshakeRequestParams
                {
                    Key = _keyPair.GetPublicKeyPem()
                },
                RequestTimeMils = 0
            };

            var httpContent = new StringContent(JsonConvert.SerializeObject(handshake), Encoding.UTF8, "application/json");
            Uri uri = new Uri($"http://{_deviceIP}/app");

            HttpResponseMessage response = await _client.PostAsync($"http://{_deviceIP}/app", httpContent);

            CookieContainer cookies = new CookieContainer();

            foreach (var cookieHeader in response.Headers.GetValues("Set-Cookie"))
            {
                cookies.SetCookies(uri, cookieHeader);
            }
            string cookieValue = cookies.GetCookies(uri).FirstOrDefault(c => c.Name == "TP_SESSIONID")?.Value;

            var dataResponse = JsonConvert.DeserializeObject<HandshakeResponse>(
                await response.Content.ReadAsStringAsync());

            byte[] deviceKeyIvBytes = _keyPair.Decrypt(dataResponse.Result.Key); /*dataResponse.Result.Key*/

            byte[] KeyArray = new byte[16];
            byte[] IVArray = new byte[16];

            Array.Copy(deviceKeyIvBytes, 0, KeyArray, 0, 16);
            Array.Copy(deviceKeyIvBytes, 16, IVArray, 0, 16);

            DeviceInfo deviceInfo = new DeviceInfo
            {
                Key = KeyArray,
                IV = IVArray,
                SessionId = cookieValue
            };

            return deviceInfo;
        }

        private async Task<string> RequestWithHeader(string uri, string json, string cookie)
        {
            string responseJson;

            var handler = new HttpClientHandler();

            handler.AutomaticDecompression = ~DecompressionMethods.None;

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), uri))
                {
                    request.Headers.TryAddWithoutValidation("Cookie", $"TP_SESSIONID={cookie}");

                    request.Content = new StringContent(json);

                    var responseRaw = await httpClient.SendAsync(request);
                    responseJson = await responseRaw.Content.ReadAsStringAsync(); //pohuy chto 2 raza await
                }
            }

            LoginDeviceResponse response = JsonConvert.DeserializeObject<LoginDeviceResponse>(responseJson);

            return response.Result.Response;
        }
    }
}
