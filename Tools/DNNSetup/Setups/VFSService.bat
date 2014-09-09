@ECHO OFF

SET BASE_PATH_VFS=%1
SET BASE_PATH_TARGET=%2

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM The following directory is for VS2012
SET VS2012=C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE
SET PATH=%PATH%;%VS2012%

SET SOLUTION_PATH_VFS="%BASE_PATH_VFS%\Services.sln"

SET ServiceName="ETSI VFS Service"
SET ServiceTargetPath="%BASE_PATH_TARGET%\VfsService\"
SET ServiceOriginPath="%BASE_PATH_VFS%\Etsi.Ngpp.VfsService.Setup\bin\Release\"

echo Step1 [VFS]: Build VFS application

echo Building VFS solution
devenv %SOLUTION_PATH_VFS% /clean Release | findstr /R /C:"===="
devenv %SOLUTION_PATH_VFS% /build Release | findstr /R /C:"===="

echo Step2[VFS]: Creating the services directory and moving the files in.
IF exist %BASE_PATH_TARGET% ( echo %BASE_PATH_TARGET% exists ) ELSE ( mkdir %BASE_PATH_TARGET% && echo %BASE_PATH_TARGET% created)

REM ----------------------------------------------------------------------------
REM                           VFS Service
REM ----------------------------------------------------------------------------
echo Step 2.1[VFS]: Installing VFS Service.

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
installutil.exe /U "Etsi.Ngpp.VfsService.Setup.exe"

REM Installing the service
installutil.exe "Etsi.Ngpp.VfsService.Setup.exe"

REM Configuring the service so that it starts automatically
sc config %ServiceName% start= auto

REM Start the service
NET START %ServiceName%

echo VFS Service installed.
echo