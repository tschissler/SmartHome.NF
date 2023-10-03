using NEnvoy;
using NEnvoy.Models;

namespace EnphaseLib
{
    public class EnphaseGatewayConnector
    {
        private static EnvoyClient client; 
        //static EnphaseConnector()
        //{
        //    //client = Login().Result;
        //}

        private static async Task<EnvoyClient> Login()
        {
            var ci = new EnvoyConnectionInfo()
            {
                Username = Secrets.EnphaseSecrets.UserName,
                Password = Secrets.EnphaseSecrets.Password,
                //EnvoyHost = "envoy.local",
                //EnphaseEntrezBaseUri = "https://entrez.enphaseenergy.com/"
            };
            return await EnvoyClient.FromLoginAsync(ci).ConfigureAwait(false);
        }

        public static IEnumerable<RootMeterReading> GetMeterReadings()
        {
            var c = Login().Result;
            Thread.Sleep(1000);
            return c.GetMeterReadingsAsync().Result;
        }

    }
}
