using System;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string FlattenExceptionMessage(this Exception ex)
        {
            string result = string.Empty;
            result += ex.Message;

            if (ex.InnerException != null)
            {
                result += " | " + ex.InnerException.FlattenExceptionMessage();               
            }
            return result;
        }
    }
}
