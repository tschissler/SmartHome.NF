using System;
using System.Text;
using System.IO;
using System.Net;
using nanoFramework.Json;
using nanoFramework.Networking;
using System.Security.Cryptography.X509Certificates;

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

            // set the headers we are going to pass a json body so use a content type of Json
            httpWebRequest.ContentType = "application/json";
            // add an Authorization header of our SAS

            // this example uses Tls 1.2 with Azure Iot Hub
            httpWebRequest.SslProtocols = System.Net.Security.SslProtocols.Tls12;

            // use the pem certificate we created earlier
            httpWebRequest.HttpsAuthentCert = rootCACert;

            var buffer = Encoding.UTF8.GetBytes($"\"UTC Timestamp: {DateTime.UtcNow} | Message: {message}\"");
            httpWebRequest.ContentLength = buffer.Length;
            httpWebRequest.GetRequestStream().Write(buffer, 0, buffer.Length);

            httpWebRequest.Dispose();
        }
    }
}
