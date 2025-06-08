
#define LYT_DEBUG 

namespace Lyt.Cleaner;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Cleaner");

#if LYT_DEBUG 
        string path = "C:\\Users\\Laurent\\Desktop";
        // string path = @"C:\Users\Laurent\Desktop\Code"; 
        // string path = @"C:\Users\Laurent\source\repos";
        string[] debugArgs = [path];
        args = debugArgs;
#endif

        var clean = new Clean(); 
        if (clean.Initialize(args))
        {
            if (clean.Run())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Clean success");
                Console.ReadLine();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Cleaner error: Failed to complete.");
            Console.ReadLine();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Failed to initialize Cleaner");
        Console.ReadLine();
    }
}
