using CookComputing.XmlRpc;

namespace PowerDog
{
    public struct Result
    {
        public string ErrorString;
        public XmlRpcStruct Reply;
        public int ErrorCode;
    }
}