namespace DataKanaEwa.Services;

/// <summary>Maps raw process names to friendly app names and categories.</summary>
public static class AppCatalog
{
    public const string Browser = "Browsers";
    public const string Social = "Social";
    public const string Games = "Games";
    public const string Media = "Media";
    public const string Dev = "Dev";
    public const string Cloud = "Cloud";
    public const string System = "System";
    public const string Other = "Other";

    private static readonly Dictionary<string, (string Display, string Category)> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["chrome"] = ("Google Chrome", Browser),
            ["msedge"] = ("Microsoft Edge", Browser),
            ["msedgewebview2"] = ("Edge WebView2", Browser),
            ["firefox"] = ("Firefox", Browser),
            ["brave"] = ("Brave", Browser),
            ["opera"] = ("Opera", Browser),
            ["vivaldi"] = ("Vivaldi", Browser),

            ["javaw"] = ("Java / Minecraft", Games),
            ["java"] = ("Java / Minecraft", Games),
            ["minecraft"] = ("Minecraft", Games),
            ["minecraft.windows"] = ("Minecraft", Games),
            ["minecraftlauncher"] = ("Minecraft", Games),
            ["tlauncher"] = ("TLauncher", Games),
            ["steam"] = ("Steam", Games),
            ["steamwebhelper"] = ("Steam", Games),
            ["steamservice"] = ("Steam", Games),
            ["epicgameslauncher"] = ("Epic Games", Games),
            ["epicwebhelper"] = ("Epic Games", Games),

            ["tiktok"] = ("TikTok", Social),
            ["tiktok live studio"] = ("TikTok", Social),
            ["facebook"] = ("Facebook", Social),
            ["messenger"] = ("Messenger", Social),
            ["whatsapp"] = ("WhatsApp", Social),
            ["whatsapp.root"] = ("WhatsApp", Social),
            ["telegram"] = ("Telegram", Social),
            ["discord"] = ("Discord", Social),
            ["zoom"] = ("Zoom", Social),
            ["teams"] = ("Microsoft Teams", Social),
            ["ms-teams"] = ("Microsoft Teams", Social),

            ["youtube"] = ("YouTube", Media),
            ["youtube music"] = ("YouTube Music", Media),
            ["spotify"] = ("Spotify", Media),
            ["vlc"] = ("VLC", Media),
            ["obs64"] = ("OBS Studio", Media),
            ["bandicam"] = ("Bandicam", Media),

            ["code"] = ("VS Code", Dev),
            ["node"] = ("Node.js", Dev),
            ["python"] = ("Python", Dev),
            ["pythonw"] = ("Python", Dev),
            ["git"] = ("Git", Dev),
            ["git-remote-https"] = ("Git", Dev),

            ["onedrive"] = ("OneDrive", Cloud),
            ["dropbox"] = ("Dropbox", Cloud),
            ["googledrivefs"] = ("Google Drive", Cloud),
            ["qbittorrent"] = ("qBittorrent", Cloud),
            ["idman"] = ("Internet Download Manager", Cloud),

            ["svchost"] = ("Windows Services", System),
            ["msmpeng"] = ("Windows Defender", System),
            ["system"] = ("System", System),
            ["unknown"] = ("Unknown", System),
        };

    private static readonly Dictionary<string, string> CategoryByDisplay = BuildCategoryIndex();

    private static Dictionary<string, string> BuildCategoryIndex()
    {
        var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var v in Map.Values) d[v.Display] = v.Category;
        return d;
    }

    public static string DisplayName(string processName)
    {
        if (string.IsNullOrWhiteSpace(processName)) return "Unknown";
        if (Map.TryGetValue(processName, out var info)) return info.Display;
        return char.ToUpperInvariant(processName[0]) + processName.Substring(1);
    }

    public static string CategoryOf(string displayName) =>
        CategoryByDisplay.TryGetValue(displayName, out var c) ? c : Other;
}
