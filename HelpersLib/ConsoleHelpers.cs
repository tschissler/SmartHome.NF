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
        
        public static void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
        
    }
}