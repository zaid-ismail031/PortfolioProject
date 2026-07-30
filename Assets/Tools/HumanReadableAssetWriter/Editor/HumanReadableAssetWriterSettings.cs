namespace Teams.Tools.HumanReadableAssetWriting
{
  public class HumanReadableAssetWriterSettings
  {
    public static HumanReadableAssetWriterSettings DefaultSettings =>
      new HumanReadableAssetWriterSettings
      {
        RoundFloatingPointNumbers = true,
        FixJsonIndentation = true,
        ResolveGuids = true,
        IncludeParticleSystems = false,
        ChangeIndentationFrom4To2Spaces = true,
        LogFileWrites = false,
        AssetPathsToIgnore = string.Empty
      };

    public bool RoundFloatingPointNumbers { get; set; }
    public bool FixJsonIndentation { get; set; }
    public bool ResolveGuids { get; set; }
    public bool IncludeParticleSystems { get; set; }
    public bool ChangeIndentationFrom4To2Spaces { get; set; }
    public bool LogFileWrites { get; set; }
    public string AssetPathsToIgnore { get; set; } = string.Empty;
  }
}
