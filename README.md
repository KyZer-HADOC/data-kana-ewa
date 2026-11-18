# Data Kana Ewa

Windows desktop app that shows **which app is using your internet data** (browsers, TikTok, WhatsApp, Minecraft, Steam, etc.) with live speed and Today / 7 days / 30 days / All-time totals.

- .NET 8 WPF GUI
- Per-process network bytes via Windows kernel ETW (TCP/UDP send + receive)
- Usage history saved in `%AppData%\DataKanaEwa\usage.json`
- Installer (`setup.exe`) built automatically by GitHub Actions

## Download

Go to **Releases** (or Actions > latest run > Artifacts) and download `DataKanaEwa-Setup-x.y.z.exe`.

Windows SmartScreen may warn because the installer is not code-signed: click *More info > Run anyway*.
The app needs **administrator rights** (required for ETW network tracing).

## Notes

- Counts app-level payload bytes, so numbers are close to (not identical to) your ISP/router numbers.
- Loopback (127.0.0.1) traffic is ignored.
- Browser traffic (YouTube, Facebook, TikTok web) is counted under the browser, because Windows only reports the process.

## Build locally

```
dotnet publish src/DataKanaEwa/DataKanaEwa.csproj -c Release -r win-x64 --self-contained true -o publish
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer\setup.iss
```
Output: `dist\DataKanaEwa-Setup-1.0.0.exe`
