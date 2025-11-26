namespace Lyt.Upgrader;

internal class Upgrade
{
    private readonly EnumerationOptions enumerationOptions;
    private string rootPath;

    //private const string find = "<TargetFramework>net9.0</TargetFramework>";
    //private const string source = "<TargetFramework>net9.0</TargetFramework>";
    //private const string target = "<TargetFramework>net10.0</TargetFramework>";

    private const string find = "<PackageReference Include=\"Avalonia";
    private const string source = "Version=\"11.3.8\"";
    private const string target = "Version=\"11.3.9\"";

    public Upgrade()
    {
        this.enumerationOptions = new EnumerationOptions()
        {
            IgnoreInaccessible = false,
            RecurseSubdirectories = false,
            MatchType = MatchType.Simple,
            MaxRecursionDepth = 8,
        };

        this.rootPath = string.Empty;
    }

    internal bool Initialize(string[] parameters)
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
        Console.WriteLine("Upgrader Path: " + this.rootPath);
        return true;
    }

    internal bool Run()
    {
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

        return success;
    }

    private bool ProcessDirectory(string folderPath)
    {
        static bool UpgradeProjectFile(string projectFile)
        {
            bool edited = false;
            string[] lines = File.ReadAllLines(projectFile);
            List<string> newLines = [];
            foreach (string line in lines)
            {
                string newLine = line;
                if ((line.Contains(find, StringComparison.InvariantCultureIgnoreCase)) &&
                    (line.Contains(source, StringComparison.InvariantCultureIgnoreCase)))
                {
                    newLine = line.Replace(source, target);
                    edited = true;
                }

                newLines.Add(newLine);
            }

            if (edited)
            {
                File.WriteAllLines(projectFile, newLines);
            }

            return edited;
        }

        try
        {
            // Check if the folder contains a *.csproj file
            var csProjFiles = folderPath.EnumerateFiles(this.enumerationOptions, "*.csproj");
            if (csProjFiles.Count > 0)
            {
                foreach (string projectFile in csProjFiles)
                {
                    if (UpgradeProjectFile(projectFile))
                    {
                        Console.WriteLine("Upgraded: " + projectFile);
                    }
                }
            }

            // Recurse to sub folders, except '.git'
            var subDirs = folderPath.EnumerateDirectories();
            foreach (string subDir in subDirs)
            {
                if (subDir.Contains(".git", StringComparison.InvariantCultureIgnoreCase))
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
