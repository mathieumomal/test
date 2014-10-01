@ECHO OFF

SET BASE_PATH_ULTIMATE_SERVICE=%~1
SET BASE_PATH_TARGET=%2

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM The following directory is for VS2012
SET VS2012=C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE
SET PATH=%PATH%;%VS2012%

SET SOLUTION_PATH_ULTIMATE_SERVICE="%BASE_PATH_ULTIMATE_SERVICE%\UltimateService.sln"

SET ServiceName="3GU Ultimate Service"
SET ServiceTargetPath="%BASE_PATH_TARGET%\UltimateService\"

echo Step1 [Ultimate Service]: Build Ultimate Service application

echo Building Ultimate Service solution
echo @@@ Enter 'd' for Debug or 'r' for release build :
set /P DEBUG_OR_RELEASE=
IF "%DEBUG_OR_RELEASE%" == "d" (
	ECHO DEBUG BUILD
	devenv %SOLUTION_PATH_ULTIMATE_SERVICE% /clean Debug | findstr /R /C:"===="
	devenv %SOLUTION_PATH_ULTIMATE_SERVICE% /build Debug | findstr /R /C:"===="
	SET ServiceOriginPath="%BASE_PATH_ULTIMATE_SERVICE%\Etsi.Ultimate.WCF.Setup\bin\Debug\"
) ELSE (
	ECHO RELEASE BUILD
	devenv %SOLUTION_PATH_ULTIMATE_SERVICE% /clean Release | findstr /R /C:"===="
	devenv %SOLUTION_PATH_ULTIMATE_SERVICE% /build Release | findstr /R /C:"===="
	SET ServiceOriginPath="%BASE_PATH_ULTIMATE_SERVICE%\Etsi.Ultimate.WCF.Setup\bin\Release\"
)


echo Step2[Ultimate Service]: Creating the services directory and moving the files in.
IF exist %BASE_PATH_TARGET% ( echo %BASE_PATH_TARGET% exists ) ELSE ( mkdir %BASE_PATH_TARGET% && echo %BASE_PATH_TARGET% created)

REM ----------------------------------------------------------------------------
REM                           Ultimate Service Service
REM ----------------------------------------------------------------------------
echo Step 2.1[Ultimate Service]: Installing Ultimate Service Service.

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
installutil.exe /U "Etsi.Ultimate.WCF.Setup.exe"

REM Installing the service
installutil.exe "Etsi.Ultimate.WCF.Setup.exe"

REM Configuring the service so that it starts automatically
sc config %ServiceName% start= auto

REM Start the service
NET START %ServiceName%

echo Ultimate Service Service installed.
echo