namespace Lyt.FileSystem;

using System.Diagnostics;

public static class FileSystemExtensions
{
    public static DriveInfo? DriveInfo(this string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            string root = directoryInfo.Root.ToString();
            if (root.EndsWith('\\'))
            {
                string driveName = root.Replace("\\", string.Empty);
                return new DriveInfo(driveName);
            }
        }

        return null;
    }

    // Available Free Space in bytes 
    public static long AvailableFreeSpace(this string folderPath)
    {
        if (DriveInfo(folderPath) is DriveInfo driveInfo)
        {
            return driveInfo.AvailableFreeSpace;
        }

        return 0;
    }


    // Enumerates files 
    public static List<string> EnumerateFiles(
        this string folderPath,
        EnumerationOptions enumerationOptions,
        string extension,
        string filter = "")
    {
        try
        {
            if (Directory.Exists(folderPath))
            {
                var files =
                    Directory.EnumerateFiles(folderPath, extension, enumerationOptions);
                var list = new List<string>(16);
                foreach (string file in files)
                {
                    if ((!string.IsNullOrWhiteSpace(filter)) &&
                        (!file.Contains(filter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        continue;
                    }

                    var fileInfo = new FileInfo(file);
                    list.Add(fileInfo.FullName);
                }

                return list;
            }
            else
            {
                Debug.WriteLine("No such directory.");
                return [];
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return [];
        }
    }

    public static List<string> EnumerateDirectories(this string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
            {

                var directories = Directory.EnumerateDirectories(folderPath);
                var list = new List<string>(16);
                foreach (string directory in directories)
                {
                    var directoryInfo = new DirectoryInfo(directory);
                    list.Add(directoryInfo.FullName);
                }

                return list;
            }
            else
            {
                Debug.WriteLine("No such directory.");
                return [];
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return [];
        }
    }

    public static string? ChangeFileExtension(this string path, string newExtension)
    {
        try
        {
            if (File.Exists(path))
            {
                string? newPath = Path.ChangeExtension(path, newExtension);
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    File.Move(path, newPath);
                    return newPath;
                }
                else
                {
                    Debug.WriteLine("Failed to swap extension");
                }
            }
            else
            {
                Debug.WriteLine("No such file.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            if ( Debugger.IsAttached) { Debugger.Break(); }
            return ex.Message;
        }

        return string.Empty;
    }


    public static bool IsReadable(this string filePath)
    {
        // First, do a quick check to avoid unnecessary exception overhead
        if (!File.Exists(filePath))
        {
            return false;
        }

        try
        {
            // Attempt to open the file strictly for reading
            using FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // The file was successfully opened and can be read
            return true;
        }
        catch (IOException ioex)
        {
            // Thrown if the file is locked by another process or a disk error occurs
            Debug.WriteLine(ioex.ToString());
            return false;
        }
        catch (UnauthorizedAccessException uaex)
        {
            // Thrown if the application lacks OS permissions to read the file
            Debug.WriteLine(uaex.ToString());
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return false;
        }
    }

    // Make sure the provided folder is writeable and readable, and that we can create
    // and delete a file in it. (Full Control)
    public static bool IsFolderFullControl(this string folderPath)
    {
        try
        {
            if( !Directory.Exists(folderPath))
            {
                return false;
            }

            const string testReadWrite = "testReadWrite";
            string testFilePath = Path.Combine(folderPath, testReadWrite + ".txt");
            File.WriteAllText(testFilePath, testReadWrite);
            string content = File.ReadAllText(testFilePath);
            bool fail = false;
            if (content != testReadWrite)
            {
                fail = true;
            }

            File.Delete(testFilePath);
            return !fail;
        }
        catch (IOException ioex)
        {
            // Thrown if the file is locked by another process or a disk error occurs
            Debug.WriteLine(ioex.ToString());
            return false;
        }
        catch (UnauthorizedAccessException uaex)
        {
            // Thrown if the application lacks OS permissions to read or write a file
            Debug.WriteLine(uaex.ToString());
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception in IsValid: " + ex.ToString());
            return false;
        }
    }

    private static readonly HashSet<string> WindowsReservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    public static string SanitizeFolderName(string input, string fallback = "Untitled", int minLength = 4, int maxLength = 20)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return fallback;
        }

        if (input.Length < minLength)
        {
            return fallback;
        }

        // Replace invalid file/folder name characters with an underscore
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string sanitized = string.Concat(input.Select(c => invalidChars.Contains(c) ? '_' : c));

        // Trim leading/trailing whitespace and trailing dots (Windows strips trailing dots/spaces)
        sanitized = sanitized.Trim().TrimEnd('.');

        // Handle empty strings or relative path segments ("." or "..")
        if (string.IsNullOrWhiteSpace(sanitized) || sanitized is "." or "..")
        {
            return fallback;
        } 

        // Prefix Windows reserved device names (e.g., "CON" or "CON.txt" -> "_CON")
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(sanitized);
        if (WindowsReservedNames.Contains(nameWithoutExtension))
        {
            sanitized = "_" + sanitized;
        }

        // Enforce a safe segment length (255 is standard max for NTFS/ext4/APFS)
        if (sanitized.Length > maxLength)
        {
            sanitized = sanitized[..maxLength].TrimEnd('.', ' ');
        }

        if (sanitized.Length < minLength)
        {
            return fallback;
        }

        return sanitized;
    }

    public static DirectoryInfo CreateSafeSubdirectory(string baseDirectory, string rawFolderName)
    {
        string safeName = SanitizeFolderName(rawFolderName);
        return CreateSafeSubdirectory(baseDirectory, safeName, rawFolderName); 
    } 

    public static DirectoryInfo CreateSafeSubdirectory(string baseDirectory, string safeName, string rawFolderName)
    { 
        string fullBasePath = Path.GetFullPath(baseDirectory);
        string targetPath = Path.GetFullPath(Path.Combine(fullBasePath, safeName));

        // Prevent path traversal attacks (ensure final path is strictly inside baseDirectory)
        string baseWithSeparator = 
            fullBasePath.EndsWith(Path.DirectorySeparatorChar)  ? 
                fullBasePath : 
                fullBasePath + Path.DirectorySeparatorChar;

        if (!targetPath.StartsWith(baseWithSeparator, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Resulting folder path escapes the base directory.");
        }

        return Directory.CreateDirectory(targetPath);
    }
}
