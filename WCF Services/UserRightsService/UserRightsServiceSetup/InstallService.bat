@ECHO OFF

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM Uninstalling service if it already existed
installutil.exe /U "Etsi.UserRights.UserRightsServiceSetup.exe"

REM Installing the service
installutil.exe "Etsi.UserRights.UserRightsServiceSetup.exe"

REM Start the service
NET START "ETSI User Rights Service"

PAUSE