using System.Diagnostics;

namespace DataKanaEwa.Services;

/// <summary>Start-with-Windows via an elevated scheduled task (the app needs admin, so a Run key would show UAC every login).</summary>
public static class AutoStart
{
    private const string TaskName = "DataKadayoo";

    public static bool IsEnabled() => Run($"/Query /TN {TaskName}") == 0;

    public static bool Set(bool enabled)
    {
        if (enabled)
        {
            var exe = Environment.ProcessPath ?? "";
            return Run($"/Create /TN {TaskName} /TR \"\\\"{exe}\\\" --tray\" /SC ONLOGON /RL HIGHEST /F") == 0;
        }
        return Run($"/Delete /TN {TaskName} /F") == 0;
    }

    private static int Run(string args)
    {
        try
        {
            var psi = new ProcessStartInfo("schtasks.exe", args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var p = Process.Start(psi);
            if (p is null) return -1;
            p.StandardOutput.ReadToEnd();
            p.StandardError.ReadToEnd();
            p.WaitForExit(10000);
            return p.HasExited ? p.ExitCode : -1;
        }
        catch
        {
            return -1;
        }
    }
}
