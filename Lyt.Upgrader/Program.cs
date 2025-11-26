#define LYT_DEBUG 

namespace Lyt.Upgrader;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Upgrader");

#if DEBUG
#if LYT_DEBUG 
        string path = "C:\\Users\\Laurent\\source\\repos";
        // string path = @"C:\Users\Laurent\Desktop\Code"; 
        string[] debugArgs = [path];
        args = debugArgs;
#endif
#endif

        var upgrade = new Upgrade();
        if (upgrade.Initialize(args))
        {
            if (upgrade.Run())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Upgrader success");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Upgrader error: Failed to complete.");
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to initialize Upgrader");
        }

        Console.ReadLine();
        Console.ForegroundColor = ConsoleColor.White;
    }
}
