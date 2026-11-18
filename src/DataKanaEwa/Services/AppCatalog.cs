namespace DataKanaEwa.Services;

/// <summary>Maps raw process names to friendly app names.</summary>
public static class AppCatalog
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["chrome"] = "Google Chrome",
        ["msedge"] = "Microsoft Edge",
        ["msedgewebview2"] = "Edge WebView2",
        ["firefox"] = "Firefox",
        ["brave"] = "Brave",
        ["opera"] = "Opera",
        ["vivaldi"] = "Vivaldi",

        ["javaw"] = "Java / Minecraft",
        ["java"] = "Java / Minecraft",
        ["minecraft"] = "Minecraft",
        ["minecraft.windows"] = "Minecraft",
        ["minecraftlauncher"] = "Minecraft",

        ["tiktok"] = "TikTok",
        ["tiktok live studio"] = "TikTok",
        ["youtube"] = "YouTube",
        ["youtube music"] = "YouTube Music",
        ["facebook"] = "Facebook",
        ["messenger"] = "Messenger",
        ["whatsapp"] = "WhatsApp",
        ["whatsapp.root"] = "WhatsApp",
        ["telegram"] = "Telegram",
        ["discord"] = "Discord",
        ["spotify"] = "Spotify",

        ["steam"] = "Steam",
        ["steamwebhelper"] = "Steam",
        ["steamservice"] = "Steam",
        ["epicgameslauncher"] = "Epic Games",
        ["epicwebhelper"] = "Epic Games",

        ["code"] = "VS Code",
        ["node"] = "Node.js",
        ["python"] = "Python",
        ["pythonw"] = "Python",
        ["git"] = "Git",
        ["git-remote-https"] = "Git",

        ["onedrive"] = "OneDrive",
        ["dropbox"] = "Dropbox",
        ["googledrivefs"] = "Google Drive",
        ["teams"] = "Microsoft Teams",
        ["ms-teams"] = "Microsoft Teams",
        ["zoom"] = "Zoom",
        ["vlc"] = "VLC",
        ["qbittorrent"] = "qBittorrent",
        ["idman"] = "Internet Download Manager",

        ["svchost"] = "Windows Services",
        ["msmpeng"] = "Windows Defender",
        ["system"] = "System",
        ["unknown"] = "Unknown",
    };

    public static string DisplayName(string processName)
    {
        if (string.IsNullOrWhiteSpace(processName)) return "Unknown";
        if (Map.TryGetValue(processName, out var name)) return name;
        return char.ToUpperInvariant(processName[0]) + processName.Substring(1);
    }
}
