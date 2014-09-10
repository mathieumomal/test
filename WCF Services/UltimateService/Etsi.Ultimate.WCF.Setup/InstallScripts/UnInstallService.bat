@ECHO OFF

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM Stop the service
NET STOP "3GU Ultimate Service"

REM Uninstalling service if it already existed
installutil.exe /U "Etsi.Ultimate.WCF.Setup.exe"

PAUSE