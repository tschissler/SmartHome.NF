namespace HelpersLib
{
    public class ConsoleHelpers
    {
        private static DateTime lastClearTimeStamp;
        
        public static void PrintConsoleOutput(int x, int y, string output)
        {
            if (DateTime.Now - lastClearTimeStamp > TimeSpan.FromSeconds(10))
            {
                Console.Clear();
                lastClearTimeStamp = DateTime.Now;
            }
            Console.SetCursorPosition(x, y);
            Console.Write(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
            Console.SetCursorPosition(x, y);
            Console.Write(output);
        }

        public static void PrintErrorMessage(string message)
        {
            Console.SetCursorPosition(0, Console.WindowHeight-1);
            Console.Write(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ResetColor();
        }
        
        public static void PrintSuccessMessage(int x, int y, string message)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ResetColor();
        }
        
    }
}