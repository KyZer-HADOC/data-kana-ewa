using System.Windows;
using DataKanaEwa.Services;

namespace DataKanaEwa;

public partial class App : Application
{
    private const string ShowEventName = "DataKadayoo_ShowWindow";
    private Mutex? _mutex;
    private EventWaitHandle? _showEvent;

    protected override void OnStartup(StartupEventArgs e)
    {
        // helper switches used by the installer
        if (e.Args.Contains("--enable-autostart")) { AutoStart.Set(true); Shutdown(); return; }
        if (e.Args.Contains("--disable-autostart")) { AutoStart.Set(false); Shutdown(); return; }

        // single instance: a second launch just brings the running (tray) instance to the front
        _mutex = new Mutex(true, "DataKadayoo_SingleInstance", out bool isFirst);
        _showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowEventName);
        if (!isFirst)
        {
            _showEvent.Set();
            Shutdown();
            return;
        }

        var waiter = new Thread(() =>
        {
            while (true)
            {
                _showEvent.WaitOne();
                Dispatcher.BeginInvoke(new Action(() =>
                    (Current.MainWindow as DataKanaEwa.MainWindow)?.ShowFromTray()));
            }
        })
        { IsBackground = true, Name = "DataKadayoo-ShowWaiter" };
        waiter.Start();

        base.OnStartup(e);
    }
}
