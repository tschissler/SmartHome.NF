using CookComputing.XmlRpc;

namespace PVController
{
    public struct Result
    {
        public string ErrorString;
        public XmlRpcStruct Reply;
        public int ErrorCode;
    }
}