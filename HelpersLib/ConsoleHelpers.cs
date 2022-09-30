namespace HelpersLib
{
    public class ConsoleHelpers
    {
        private static DateTime lastClearTimeStamp;
        
        public static void PrintConsoleOutput(int x, int y, string output)
        {
            return;
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
            return;
            Console.SetCursorPosition(0, Console.WindowHeight-1);
            Console.Write(String.Concat(Enumerable.Repeat(" ", Console.WindowWidth)));
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
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