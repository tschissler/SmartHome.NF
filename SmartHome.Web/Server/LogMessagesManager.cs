namespace SmartHome.Web.Server
{
    public class LogMessagesManager
    {
        private static Queue<string> _logMessages = new Queue<string>();

        public static void Enqueue(string message)
        {
            if (_logMessages.Count > 1000)
            {
                _logMessages.Dequeue();
            }
            _logMessages.Enqueue(message);
        }

        public static string[] ReadLogMessages()
        {
            return _logMessages.ToArray();
        }
    }
}
