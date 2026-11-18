namespace DataKanaEwa.Services;

/// <summary>System tray icon: open window, toggle start-with-Windows, exit.</summary>
public sealed class TrayService : IDisposable
{
    private readonly System.Windows.Forms.NotifyIcon _icon;
    private readonly System.Windows.Forms.ToolStripMenuItem _autoStart;

    public TrayService(Action onOpen, Action onExit)
    {
        var menu = new System.Windows.Forms.ContextMenuStrip();
        menu.Items.Add("Open Data Kadayoo", null, (_, _) => onOpen());

        _autoStart = new System.Windows.Forms.ToolStripMenuItem("Start with Windows")
        {
            Checked = AutoStart.IsEnabled(),
        };
        _autoStart.Click += (_, _) =>
        {
            AutoStart.Set(!_autoStart.Checked);
            _autoStart.Checked = AutoStart.IsEnabled();
        };
        menu.Items.Add(_autoStart);
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => onExit());

        _icon = new System.Windows.Forms.NotifyIcon
        {
            Icon = System.Drawing.Icon.ExtractAssociatedIcon(Environment.ProcessPath!),
            Text = "Data Kadayoo - counting data usage",
            Visible = true,
            ContextMenuStrip = menu,
        };
        _icon.DoubleClick += (_, _) => onOpen();
    }

    public void Notify(string title, string text) =>
        _icon.ShowBalloonTip(3000, title, text, System.Windows.Forms.ToolTipIcon.Info);

    public void Dispose()
    {
        _icon.Visible = false;
        _icon.Dispose();
    }
}
