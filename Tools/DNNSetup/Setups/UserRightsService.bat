@ECHO OFF

SET BASE_PATH_USERRIGHTS=%~1
SET BASE_PATH_TARGET=%2
SET SERVICE_LOGIN=%3
SET SERVICE_PASSWORD=%4

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM The following directory is for VS2013
SET VS2012=C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE
SET PATH=%PATH%;%VS2012%

SET SOLUTION_PATH_USERRIGHTS="%BASE_PATH_USERRIGHTS%\UserRightsService.sln"

SET ServiceName="ETSI User Rights Service"
SET ServiceTargetPath="%BASE_PATH_TARGET%\UserRightsService\"
SET ServiceOriginPath="%BASE_PATH_USERRIGHTS%\UserRightsServiceSetup\bin\Release\"

echo Step1 [UserRights]: Build UserRights application

echo Building User Rights solution
devenv %SOLUTION_PATH_USERRIGHTS% /clean Release | findstr /R /C:"===="
devenv %SOLUTION_PATH_USERRIGHTS% /build Release | findstr /R /C:"===="

echo Step2[UserRights]: Creating the services directory and moving the files in.
IF exist %BASE_PATH_TARGET% ( echo %BASE_PATH_TARGET% exists ) ELSE ( mkdir %BASE_PATH_TARGET% && echo %BASE_PATH_TARGET% created)

REM ----------------------------------------------------------------------------
REM                           User Rights Service
REM ----------------------------------------------------------------------------
echo Step 2.1[UserRights]: Installing User Rights Service.

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
installutil.exe /U "Etsi.UserRights.UserRightsServiceSetup.exe"

REM Installing the service
installutil.exe "Etsi.UserRights.UserRightsServiceSetup.exe"

REM Configuring the service so that it starts automatically
IF not exist %SERVICE_PASSWORD% (
	ECHO @@@ Please enter your password (password is required for install this service)
	set /P SERVICE_PASSWORD=
	CLS
)
sc config %ServiceName% obj= %SERVICE_LOGIN% password= %SERVICE_PASSWORD% start= auto

REM Start the service
NET START %ServiceName%

echo User Rights Service installed.
echo