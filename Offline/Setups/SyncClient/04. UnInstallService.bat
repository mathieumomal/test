@ECHO OFF

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

ECHO Uninstalling SyncClient...
ECHO ---------------------------------------------------
InstallUtil /u SyncClient.exe
ECHO ---------------------------------------------------
ECHO Uninstall completed successfully.
PAUSE