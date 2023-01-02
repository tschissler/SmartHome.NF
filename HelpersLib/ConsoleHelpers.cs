using System.Diagnostics;

namespace HelpersLib
{
    public class ConsoleHelpers
    {
        static ConsoleHelpers()
        {
            System.Diagnostics.Trace.Listeners.Add(new ConsoleTraceListener());
        }

        public static void PrintErrorMessage(string message)
        {
            System.Diagnostics.Trace.TraceError($"##{GetTimeStampText()} - {message}");
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine($"## Error: {GetTimeStampText()} - {message}");
            //Console.ResetColor();
        }

        public static void PrintInformation(string message)
        {
            System.Diagnostics.Trace.TraceInformation($"{GetTimeStampText()} - {message}");

            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine($"## Information: {GetTimeStampText()} - {message}");
            //Console.ResetColor();
        }

        public static void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }

        private static string GetTimeStampText()
        {
            return $"{DateTime.Now.ToShortDateString()}-{DateTime.Now.ToLongTimeString()}";
        }
    }
}