using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

namespace DataKanaEwa.Services;

/// <summary>
/// Listens to the Windows kernel network ETW events (TCP/UDP send + receive)
/// and accumulates bytes per process name. Needs administrator rights.
/// </summary>
public sealed class NetworkMonitor : IDisposable
{
    private const string SessionName = "DataKanaEwaNetSession";

    private readonly object _lock = new();
    private Dictionary<string, (long Down, long Up)> _pending = new();
    private readonly ConcurrentDictionary<int, string> _names = new();
    private TraceEventSession? _session;
    private Thread? _thread;

    public string? Error { get; private set; }
    public bool IsRunning { get; private set; }

    public void Start()
    {
        _thread = new Thread(Run) { IsBackground = true, Name = "DataKanaEwa-ETW" };
        _thread.Start();
    }

    private void Run()
    {
        try
        {
            try { TraceEventSession.GetActiveSession(SessionName)?.Stop(); } catch { }

            _session = new TraceEventSession(SessionName);
            _session.EnableKernelProvider(KernelTraceEventParser.Keywords.NetworkTCPIP);
            _session.Source.Kernel.All += OnEvent;
            IsRunning = true;
            _session.Source.Process(); // blocks until the session is disposed
        }
        catch (UnauthorizedAccessException)
        {
            Error = "Administrator access is required to monitor network usage. Run the app as administrator.";
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            IsRunning = false;
        }
    }

    private void OnEvent(TraceEvent data)
    {
        try
        {
            var n = data.EventName;
            if (!n.StartsWith("TcpIp", StringComparison.Ordinal) &&
                !n.StartsWith("UdpIp", StringComparison.Ordinal)) return;

            bool down = n.Contains("Recv", StringComparison.Ordinal) ||
                        n.Contains("Receive", StringComparison.Ordinal);
            bool up = !down && n.Contains("Send", StringComparison.Ordinal);
            if (!down && !up) return;

            var sizeObj = data.PayloadByName("size");
            if (sizeObj is null) return;
            long size = Convert.ToInt64(sizeObj);
            if (size <= 0) return;

            // ignore local loopback traffic (local servers, bots, etc.)
            if (data.PayloadByName("daddr") is IPAddress ip && IPAddress.IsLoopback(ip)) return;

            Add(data.ProcessID, size, down);
        }
        catch
        {
            // never let one bad event kill the session
        }
    }

    private void Add(int pid, long size, bool down)
    {
        var name = _names.GetOrAdd(pid, Resolve);
        lock (_lock)
        {
            _pending.TryGetValue(name, out var cur);
            _pending[name] = down ? (cur.Down + size, cur.Up) : (cur.Down, cur.Up + size);
        }
    }

    private static string Resolve(int pid)
    {
        if (pid == 0 || pid == 4) return "System";
        try
        {
            using var p = Process.GetProcessById(pid);
            return p.ProcessName;
        }
        catch
        {
            return "Unknown";
        }
    }

    /// <summary>Returns the bytes collected since the last call and resets the counters.</summary>
    public Dictionary<string, (long Down, long Up)> Drain()
    {
        lock (_lock)
        {
            var result = _pending;
            _pending = new();
            return result;
        }
    }

    public void Dispose()
    {
        try { _session?.Dispose(); } catch { }
    }
}
