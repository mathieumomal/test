@ECHO OFF

SET BASE_PATH_FTP=%1
SET BASE_PATH_TARGET=%2

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM The following directory is for VS2012
SET VS2012=C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE
SET PATH=%PATH%;%VS2012%

SET SOLUTION_PATH_FTP="%BASE_PATH_FTP%\Services.sln"

SET ServiceName="ETSI FTP Service"
SET ServiceTargetPath="%BASE_PATH_TARGET%\FtpService\"
SET ServiceOriginPath="%BASE_PATH_FTP%\Etsi.Ngpp.FtpService.Setup\bin\Release\"

echo Step1 [FTP]: Build FTP application

echo Building FTP solution
devenv %SOLUTION_PATH_FTP% /clean Release | findstr /R /C:"===="
devenv %SOLUTION_PATH_FTP% /build Release | findstr /R /C:"===="

echo Step2[FTP]: Creating the services directory and moving the files in.
IF exist %BASE_PATH_TARGET% ( echo %BASE_PATH_TARGET% exists ) ELSE ( mkdir %BASE_PATH_TARGET% && echo %BASE_PATH_TARGET% created)

REM ----------------------------------------------------------------------------
REM                           FTP Service
REM ----------------------------------------------------------------------------
echo Step 2.1[FTP]: Installing FTP Service.

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
installutil.exe /U "Etsi.Ngpp.FtpService.Setup.exe"

REM Installing the service
installutil.exe "Etsi.Ngpp.FtpService.Setup.exe"

REM Configuring the service so that it starts automatically
sc config %ServiceName% start= auto

REM Start the service
NET START %ServiceName%

echo FTP Service installed.
echo