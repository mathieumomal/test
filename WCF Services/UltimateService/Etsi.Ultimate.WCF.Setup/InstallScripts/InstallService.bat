@ECHO OFF

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM Uninstalling service if it already existed
installutil.exe /U "Etsi.Ultimate.WCF.Setup.exe"

REM Installing the service
installutil.exe "Etsi.Ultimate.WCF.Setup.exe"

REM Start the service
NET START "3GU Ultimate Service"

PAUSE