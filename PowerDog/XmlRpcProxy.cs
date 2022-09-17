using CookComputing.XmlRpc;

namespace PowerDog
{
    public class XmlRpcProxy : XmlRpcClientProtocol
    {
        [XmlRpcMethod("getPowerDogInfo")]
        public Result getPowerDogInfo(string password)
        {
            return (Result)Invoke("getPowerDogInfo", new object[] { password });
        }

        [XmlRpcMethod("getAllCurrentLinearValues")]
        public Result getAllCurrentLinearValues(string password)
        {
            return (Result)Invoke("getAllCurrentLinearValues", new object[] { password });
        }
    }
}