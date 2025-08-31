namespace Lyt.Cleaner;

internal class Clean
{
    private const long bigFileMB = 100; // 100 MB 
    private const long oneMB = 1_024 * 1_024; // 1 MB 
    private const long bigFileSize = bigFileMB * oneMB; // 100 MB 

    private readonly List<BigFile> bigFiles;
    private readonly EnumerationOptions enumerationOptions;
    private readonly string[] tempFoldersEndings =
        [
            @"AppData\Local\Temp",
            @"AppData\Local\CrashDumps",
            @"AppData\Local\SourceServer",
            @"AppData\LocalLow\Temp",
            @"AppData\Roaming\Temp",
        ];

    private string rootPath;

    public Clean()
    {
        this.rootPath = @"C:\Users";
        this.bigFiles = [];
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
            this.ListBigFiles(); 
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
            // Delete if this is the nuget cache 
            if (folderPath.EndsWith(".nuget"))
            {
                Directory.Delete(folderPath, recursive: true);
                return true;
            }

            bool isTempFolder = false;
            foreach (string ending in tempFoldersEndings)
            {
                if (folderPath.EndsWith(ending))
                {
                    isTempFolder = true;
                    this.ProcessTempFolder(folderPath); 
                } 
            }

            if (isTempFolder)
            {
                return true; 
            } 

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

            // Delete all PDB files
            // Assuming we do not care about Protein Data Bank files (Structural Biology)
            var pdbFiles = folderPath.EnumerateFiles(this.enumerationOptions, "*.pdb");
            foreach (string filePath in pdbFiles)
            {
                // Visual Studio has some PDB's that are protected and access restricted 
                // So silently swallow any exceptions
                try
                {
                    File.Delete(filePath);
                } 
                catch { /* Swallow */ }
            }

            // Check for big files 
            var allFiles = folderPath.EnumerateFiles(this.enumerationOptions, "*.*");
            if (allFiles.Count > 0)
            {
                foreach (string filePath in allFiles)
                {
                    FileInfo fileInfo = new (filePath);
                    long size = fileInfo.Length;
                    if (size > bigFileSize)
                    {
                        this.bigFiles.Add(new BigFile(filePath,size));
                    } 
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

    private void ProcessTempFolder(string folderPath)
    {
        var allFiles = folderPath.EnumerateFiles(this.enumerationOptions, "*.*");
        if (allFiles.Count > 0)
        {
            foreach (string filePath in allFiles)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { /* Swallow */ }
            }
        }

        var subDirs = folderPath.EnumerateDirectories();
        if ( subDirs.Count > 0)
        {
            foreach (string subDir in subDirs)
            {
                this.ProcessTempFolder(subDir);
                try
                {
                    Directory.Delete(subDir, recursive: true);
                }
                catch { /* Swallow */ }
            }
        }
    }

    private void ListBigFiles ()
    {
        int count = this.bigFiles.Count; 
        if (count == 0)
        {
            Console.WriteLine("No files above " + bigFileMB + " MB.");
            return;
        }

        Console.WriteLine(count + " files above " + bigFileMB + " MB.");
        List<BigFile> sorted =
            [.. (from  bigFile in this.bigFiles
             orderby bigFile.Size descending 
             select bigFile)];
        Console.WriteLine("Top largest files: ");
        int min = Math.Min(12, count);
        for (int i = 0; i < min; ++i)
        {
            BigFile bigFile = sorted[i];
            Console.WriteLine(bigFile.Path + ": " + bigFile.Size / oneMB + " MB");
        }
    }
}
