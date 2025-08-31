
#define LYT_DEBUG 

namespace Lyt.Cleaner;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Cleaner");

#if DEBUG
#if LYT_DEBUG 
        string path = "C:\\Users\\Laurent";
        // string path = @"C:\Users\Laurent\Desktop\Code"; 
        // string path = @"C:\Users\Laurent\source\repos";
        string[] debugArgs = [path];
        args = debugArgs;
#endif
#endif

        var clean = new Clean(); 
        if (clean.Initialize(args))
        {
            if (clean.Run())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Clean success");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cleaner error: Failed to complete.");
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed to initialize Cleaner");
        }

        Console.ReadLine();
        Console.ForegroundColor = ConsoleColor.White;
    }
}
