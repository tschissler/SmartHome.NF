namespace HelpersLib
{
    public class ConsoleHelpers
    {
        
        public static void PrintErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("## Error: " + message);
            Console.ResetColor();
        }

        public static void PrintInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("## Information: " + message);
            Console.ResetColor();
        }

        public static void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
        
    }
}