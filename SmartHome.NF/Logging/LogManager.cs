using System;
using System.Text;
using System.IO;
using System.Net;
using nanoFramework.Json;
using nanoFramework.Networking;
using System.Security.Cryptography.X509Certificates;
using ExtensionMethods;
using System.Diagnostics;
using System.Net.Http;

namespace SmartHome.NF.Logging
{
    public class LogManager
    {
        public static void SendLogMessage(string url, string message)
        {
            // this cert should be used when connecting to Azure IoT on the Azure Cloud available globally. Additional certs can be found in the link above
            X509Certificate rootCACert = new X509Certificate(SmartHome.NF.Resources.GetBytes(SmartHome.NF.Resources.BinaryResources.AzureRoot));

            // perform the request as a HttpWebRequest
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = false;

            // set the headers we are going to pass a json body so use a content type of Json
            httpWebRequest.ContentType = "application/json";
            // add an Authorization header of our SAS

            // this example uses Tls 1.2 with Azure Iot Hub
            httpWebRequest.SslProtocols = System.Net.Security.SslProtocols.Tls12;

            // use the pem certificate we created earlier
            httpWebRequest.HttpsAuthentCert = rootCACert;

            var buffer = Encoding.UTF8.GetBytes($"\"UTC Timestamp: {DateTime.UtcNow} | Message: {message}\"");
            httpWebRequest.ContentLength = buffer.Length;
            try
            {
                httpWebRequest.GetRequestStream().Write(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {

            }

            httpWebRequest.Dispose();
        }

        public static void SendLogException(string url, Exception ex)
        {
            SendLogMessage(url, ex.FlattenExceptionMessage() + "\n\n" + ex.StackTrace);
        }

        public static HttpStatusCode PingLogService(string url)
        {
            var responseCode = HttpStatusCode.NotFound;

            try
            {
                HttpClient httpClient = new();
                var responseMessage = httpClient.Get(url);
                responseCode = responseMessage.StatusCode;
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Debug.WriteLine("Error posting sensor data: " + responseMessage.StatusCode + " - " + responseMessage.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while reaching ping service: " + ex.Message);
            }

            return responseCode;
        }
    }
}
