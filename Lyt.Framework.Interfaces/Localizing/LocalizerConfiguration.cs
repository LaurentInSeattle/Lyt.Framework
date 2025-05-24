namespace Lyt.Framework.Interfaces.Localizing;

public sealed class LocalizerConfiguration
{
    public HashSet<string> Languages { get; set; } = []; // Example: = ["en-US", "fr-FR", "it-IT"];

    public string AssemblyName { get; set; } = string.Empty; // Example: = "TextoCopier";

    public string AssetsFolder { get; set; } = "Assets";

    public string LanguagesSubFolder { get; set; } = "Languages";

    public string LanguagesFilePrefix { get; set; } = "Lang_";

    public string LanguagesFileExtension { get; set; } = ".axaml"; // includes dot

    public bool IsLikelyValid =>
        this.Languages.Count > 0 &&
        !string.IsNullOrWhiteSpace(this.AssemblyName) &&
        !string.IsNullOrWhiteSpace(this.AssetsFolder) &&
        !string.IsNullOrWhiteSpace(this.LanguagesSubFolder) &&
        !string.IsNullOrWhiteSpace(this.LanguagesFilePrefix);

    // TODO: These two utilities do not belong here (Avalonia specific) 
    public string ResourceFileUriString(string targetLanguage)
        => string.Format(
            "avares://{0}/{1}/{2}/{3}{4}{5}",
            this.AssemblyName, this.AssetsFolder,
            this.LanguagesSubFolder, this.LanguagesFilePrefix,
            targetLanguage, this.LanguagesFileExtension);

    public string ResourceFolderUriString()
        => string.Format(
            "avares://{0}/{1}/{2}", this.AssemblyName, this.AssetsFolder, this.LanguagesSubFolder);
}
