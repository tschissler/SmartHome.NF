namespace HelpersLib
{
    public class ConsoleHelpers
    {
        private static DateTime lastClearTimeStamp;
        
        public static void PrintErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("## Error: " + message);
            Console.ResetColor();
        }
        
        public static void PrintSuccessMessage(int x, int y, string message)
        {
            return;
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ResetColor();
        }
        
    }
}