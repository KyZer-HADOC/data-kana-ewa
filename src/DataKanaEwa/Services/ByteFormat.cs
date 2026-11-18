namespace DataKanaEwa.Services;

public static class ByteFormat
{
    private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB" };

    public static string Size(long bytes)
    {
        double v = bytes;
        int i = 0;
        while (v >= 1024 && i < Units.Length - 1)
        {
            v /= 1024;
            i++;
        }
        return i == 0 ? $"{v:0} B" : $"{v:0.0} {Units[i]}";
    }

    public static string Speed(long bytesPerSecond) => Size(bytesPerSecond) + "/s";
}
