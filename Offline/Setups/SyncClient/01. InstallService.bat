@ECHO OFF

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

ECHO Installing SyncClient...
ECHO ---------------------------------------------------
InstallUtil /i SyncClient.exe
ECHO ---------------------------------------------------
ECHO Installation completed successfully.
PAUSE