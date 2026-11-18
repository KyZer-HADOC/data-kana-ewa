#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#define MyAppName "Data Kadayoo"
#define MyAppExe "DataKadayoo.exe"

[Setup]
AppId={{B7C2E8A4-5D1F-4A6B-9E3C-7F2A1D4B8C90}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=KyZer_HADOC
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\dist
OutputBaseFilename=DataKadayoo-Setup-{#MyAppVersion}
SetupIconFile=..\src\DataKanaEwa\Assets\app.ico
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExe}

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"

[InstallDelete]
Type: files; Name: "{app}\DataKanaEwa.exe"
Type: files; Name: "{autodesktop}\Data Kana Ewa.lnk"
Type: files; Name: "{autoprograms}\Data Kana Ewa.lnk"

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExe}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent shellexec
