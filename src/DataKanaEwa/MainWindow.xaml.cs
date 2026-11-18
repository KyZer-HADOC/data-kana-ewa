using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using DataKanaEwa.Models;
using DataKanaEwa.Services;

namespace DataKanaEwa;

public partial class MainWindow : Window
{
    private readonly NetworkMonitor _monitor = new();
    private readonly UsageStore _store = new();
    private readonly ObservableCollection<AppRow> _rows = new();
    private readonly Dictionary<string, AppRow> _index = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly ICollectionView _view;
    private string _period = "today";
    private int _ticks;
    private bool _ready;

    public MainWindow()
    {
        InitializeComponent();

        RowsHost.ItemsSource = _rows;
        _view = CollectionViewSource.GetDefaultView(_rows);
        _view.SortDescriptions.Add(new SortDescription(nameof(AppRow.Total), ListSortDirection.Descending));
        if (_view is ICollectionViewLiveShaping live)
        {
            live.IsLiveSorting = true;
            live.LiveSortingProperties.Add(nameof(AppRow.Total));
        }
        _view.Filter = FilterRow;
        _ready = true;

        _monitor.Start();
        _timer.Tick += OnTick;
        _timer.Start();

        Closing += (_, _) =>
        {
            _timer.Stop();
            _store.Save();
            _monitor.Dispose();
        };

        Refresh();
    }

    private bool FilterRow(object o)
    {
        var q = Search.Text?.Trim();
        return o is AppRow r && (string.IsNullOrEmpty(q) || r.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var delta = _monitor.Drain();
        long down = 0, up = 0;
        var day = DateTime.Now.ToString("yyyy-MM-dd");

        foreach (var (proc, v) in delta)
        {
            _store.Add(day, AppCatalog.DisplayName(proc), v.Down, v.Up);
            down += v.Down;
            up += v.Up;
        }

        SpeedDown.Text = ByteFormat.Speed(down);
        SpeedUp.Text = ByteFormat.Speed(up);

        if (++_ticks % 15 == 0) _store.Save();
        Refresh();

        if (_monitor.Error is { } err)
        {
            StatusText.Text = "Error: " + err;
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x8B));
        }
    }

    private void Refresh()
    {
        DateTime? from = _period switch
        {
            "today" => DateTime.Today,
            "week" => DateTime.Today.AddDays(-6),
            "month" => DateTime.Today.AddDays(-29),
            _ => null,
        };

        var agg = _store.Aggregate(from);
        long totalDown = 0, totalUp = 0;
        foreach (var p in agg.Values)
        {
            totalDown += p.Down;
            totalUp += p.Up;
        }
        long all = totalDown + totalUp;

        foreach (var (name, pair) in agg)
        {
            if (!_index.TryGetValue(name, out var row))
            {
                row = new AppRow(name);
                _index[name] = row;
                _rows.Add(row);
            }
            row.Update(pair.Down, pair.Up, all == 0 ? 0 : (pair.Down + pair.Up) * 100.0 / all);
        }

        foreach (var key in _index.Keys.Where(k => !agg.ContainsKey(k)).ToList())
        {
            _rows.Remove(_index[key]);
            _index.Remove(key);
        }

        TotalDown.Text = ByteFormat.Size(totalDown);
        TotalUp.Text = ByteFormat.Size(totalUp);
        AppCount.Text = $"{agg.Count} apps";
        EmptyText.Visibility = _rows.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Period_Checked(object sender, RoutedEventArgs e)
    {
        if (!_ready) return;
        if (sender is RadioButton { Tag: string tag })
        {
            _period = tag;
            Refresh();
        }
    }

    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_ready) _view.Refresh();
    }
}
