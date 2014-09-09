@ECHO OFF

SET BASE_PATH_SYNC_CLIENT=%1
SET BASE_PATH_TARGET=%2
SET SERVICE_LOGIN=%3
SET SERVICE_PASSWORD=%4

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM The following directory is for VS2012
SET VS2012=C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE
SET PATH=%PATH%;%VS2012%

SET SOLUTION_PATH_SYNC_CLIENT="%BASE_PATH_SYNC_CLIENT%\OfflineSync.sln"

SET ServiceName="3GU Synchronization Client"
SET ServiceTargetPath="%BASE_PATH_TARGET%\OfflineSyncClient\"
SET ServiceOriginPath="%BASE_PATH_SYNC_CLIENT%\Offline\SyncClient\bin\Release\"

echo Step1 [Sync Client]: Build Sync Client application

echo Building Sync Client solution
devenv %SOLUTION_PATH_SYNC_CLIENT% /clean Release | findstr /R /C:"===="
devenv %SOLUTION_PATH_SYNC_CLIENT% /build Release | findstr /R /C:"===="

echo Step2[Sync Client]: Creating the services directory and moving the files in.
IF exist %BASE_PATH_TARGET% ( echo %BASE_PATH_TARGET% exists ) ELSE ( mkdir %BASE_PATH_TARGET% && echo %BASE_PATH_TARGET% created)

REM ----------------------------------------------------------------------------
REM                           Sync Client
REM ----------------------------------------------------------------------------
echo Step 2.1[Sync Client]: Installing Sync Client.

REM Stop service
NET STOP %ServiceName%

REM Empty target folder (remove and recreate it)
RMDIR /S /Q %ServiceTargetPath%
mkdir %ServiceTargetPath%

REM Copy files that have been compiled to new folder
chdir /D %ServiceOriginPath% 
xcopy /S * %ServiceTargetPath%

REM Reinstall service
chdir /D %ServiceTargetPath%

REM Uninstalling service if it already existed
installutil.exe /U "SyncClient.exe"

REM Installing the service
installutil.exe "SyncClient.exe"

REM Configuring the service so that it starts automatically
sc config %ServiceName% obj= %SERVICE_LOGIN% password= %SERVICE_PASSWORD% start= auto

REM Start the service
NET START %ServiceName%

echo VFS Service installed.
echo