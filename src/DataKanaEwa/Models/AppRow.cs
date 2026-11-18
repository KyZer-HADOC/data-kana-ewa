using System.ComponentModel;
using System.Windows.Media;
using DataKanaEwa.Services;

namespace DataKanaEwa.Models;

public sealed class AppRow : INotifyPropertyChanged
{
    private static readonly (byte R, byte G, byte B)[] Palette =
    {
        (0x4F, 0x8C, 0xFF), (0x3D, 0xDC, 0x97), (0xFF, 0xB4, 0x54), (0xB7, 0x7B, 0xFF),
        (0xFF, 0x6B, 0x8B), (0x2E, 0xC4, 0xD6), (0xE0, 0xC0, 0x3A), (0x8F, 0xB8, 0x4A),
    };

    private long _down;
    private long _up;
    private double _share;

    public AppRow(string name)
    {
        Name = name;
        Initial = name.Length > 0 ? name.Substring(0, 1).ToUpperInvariant() : "?";

        int sum = 0;
        foreach (var c in name) sum += c;
        var (r, g, b) = Palette[sum % Palette.Length];
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        Accent = brush;
    }

    public string Name { get; }
    public string Initial { get; }
    public Brush Accent { get; }

    public long Total => _down + _up;
    public double Share => _share;
    public string DownText => ByteFormat.Size(_down);
    public string UpText => ByteFormat.Size(_up);
    public string TotalText => ByteFormat.Size(Total);
    public string ShareText => $"{_share:0.#}%";

    public void Update(long down, long up, double share)
    {
        if (down == _down && up == _up && Math.Abs(share - _share) < 0.05) return;
        _down = down;
        _up = up;
        _share = share;
        Raise(nameof(Total));
        Raise(nameof(Share));
        Raise(nameof(DownText));
        Raise(nameof(UpText));
        Raise(nameof(TotalText));
        Raise(nameof(ShareText));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Raise(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
