using System.IO;
using System.Globalization;
using System.Text.Json;

namespace DataKanaEwa.Services;

public sealed class UsagePair
{
    public long Down { get; set; }
    public long Up { get; set; }
}

/// <summary>Persists per-day, per-app usage totals as JSON in %AppData%\DataKanaEwa.</summary>
public sealed class UsageStore
{
    private const string DayFormat = "yyyy-MM-dd";
    private readonly string _path;
    private Dictionary<string, Dictionary<string, UsagePair>> _days = new();
    private bool _dirty;

    public UsageStore()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DataKanaEwa");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "usage.json");
        Load();
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(_path)) return;
            var json = File.ReadAllText(_path);
            _days = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, UsagePair>>>(json) ?? new();
        }
        catch
        {
            try { File.Copy(_path, _path + ".bak", true); } catch { }
            _days = new();
        }
    }

    public void Add(string day, string app, long down, long up)
    {
        if (down == 0 && up == 0) return;
        if (!_days.TryGetValue(day, out var apps)) _days[day] = apps = new();
        if (!apps.TryGetValue(app, out var pair)) apps[app] = pair = new UsagePair();
        pair.Down += down;
        pair.Up += up;
        _dirty = true;
    }

    /// <summary>Sums usage per app from the given date (inclusive) up to now. Null = all time.</summary>
    public Dictionary<string, UsagePair> Aggregate(DateTime? from)
    {
        var result = new Dictionary<string, UsagePair>();
        foreach (var (dayKey, apps) in _days)
        {
            if (from.HasValue &&
                DateTime.TryParseExact(dayKey, DayFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) &&
                d < from.Value.Date)
                continue;

            foreach (var (app, pair) in apps)
            {
                if (!result.TryGetValue(app, out var total)) result[app] = total = new UsagePair();
                total.Down += pair.Down;
                total.Up += pair.Up;
            }
        }
        return result;
    }

    public void Save()
    {
        if (!_dirty) return;
        try
        {
            var cutoff = DateTime.Today.AddDays(-400);
            foreach (var key in _days.Keys.ToList())
            {
                if (DateTime.TryParseExact(key, DayFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) &&
                    d < cutoff)
                    _days.Remove(key);
            }

            var tmp = _path + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(_days));
            File.Move(tmp, _path, true);
            _dirty = false;
        }
        catch
        {
            // try again on the next save
        }
    }
}
