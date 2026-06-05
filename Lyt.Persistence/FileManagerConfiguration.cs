namespace Lyt.Persistence;

public sealed class FileManagerConfiguration(
    string organization, string application, string rootNamespace, 
    string assemblyName, string assetsFolder, List<string>? userFolders = null)
{
    public string Organization { get; private set; } = organization;

    public string Application { get; private set; } = application;

    public string RootNamespace { get; private set; } = rootNamespace;

    public string AssemblyName { get; set; } = assemblyName; // Example: = "TextoCopier";

    public string AssetsFolder { get; set; } = assetsFolder; // Example: = "Assets";

    public List<string>? UserFolders { get; set; } = userFolders; // Example: = ["Projects"] or null;

    public string AvaresUriString() => string.Format("avares://{0}/{1}/", this.AssemblyName, this.AssetsFolder);

}
