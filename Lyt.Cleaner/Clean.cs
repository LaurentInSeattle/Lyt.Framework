namespace Lyt.Cleaner;

internal class Clean
{
    private readonly EnumerationOptions enumerationOptions;
    private string rootPath;

    public Clean()
    {
        this.rootPath = @"C:\Users";
        this.enumerationOptions = new EnumerationOptions()
        {
            IgnoreInaccessible = false,
            RecurseSubdirectories = false,
            MatchType = MatchType.Simple,
            MaxRecursionDepth = 8,
        };
    }

    public bool Initialize(string[]? parameters)
    {
        if (parameters is null || parameters.Length == 0)
        {
            return false;
        }

        string maybeRootPath = parameters[0];
        if (string.IsNullOrWhiteSpace(maybeRootPath) || !Path.Exists(maybeRootPath))
        {
            return false;
        }

        this.rootPath = maybeRootPath;
        return true;
    }

    public bool Run()
    {
        long available = this.rootPath.AvailableFreeSpace() / (1024 * 1024);
        Console.WriteLine("Available Free Space: " + available + " MB");

        bool success;
        try
        {
            success = this.ProcessDirectory(this.rootPath);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Exception thrown: \n" + ex);
            success = false;
        }

        available = this.rootPath.AvailableFreeSpace() / (1024 * 1024);
        Console.WriteLine("Available Free Space: " + available + " MB");
        return success;
    }

    private bool ProcessDirectory(string folderPath)
    {
        try
        {
            // Check if the folder contains a *.sln file 
            // If yes, delete the '.vs' directory 
            var slnFiles = folderPath.EnumerateFiles(this.enumerationOptions, "*.sln");
            if (slnFiles.Count > 0)
            {
                string vsPath = Path.Combine(folderPath, ".vs");
                if (Directory.Exists(vsPath))
                {
                    Directory.Delete(vsPath, recursive: true);
                }
            }

            // Check if the folder contains a *.csproj file
            // If yes, delete the 'obj' and 'bin' directories
            var csProjFiles = folderPath.EnumerateFiles(this.enumerationOptions, "*.csproj");
            if (csProjFiles.Count > 0)
            {
                string objPath = Path.Combine(folderPath, "obj");
                if (Directory.Exists(objPath))
                {
                    Directory.Delete(objPath, recursive: true);
                }

                string binPath = Path.Combine(folderPath, "bin");
                if (Directory.Exists(binPath))
                {
                    Directory.Delete(binPath, recursive: true);
                }
            }

            // Recurse to sub folders, except '.git'
            var subDirs = folderPath.EnumerateDirectories();
            foreach (string subDir in subDirs)
            {
                if( subDir.Contains(".git", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                this.ProcessDirectory(subDir);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception thrown: \n" + ex);
            throw;
        }

        return true;
    }
}
