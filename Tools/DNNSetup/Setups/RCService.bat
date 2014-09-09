@ECHO OFF

SET BASE_PATH_RC=%1
SET BASE_PATH_TARGET=%2

REM The following directory is for .NET 4.0
SET DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
SET PATH=%PATH%;%DOTNETFX4%

REM The following directory is for VS2012
SET VS2012=C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE
SET PATH=%PATH%;%VS2012%

SET SOLUTION_PATH_RC="%BASE_PATH_RC%\Capgemini.Etsi.RemoteConsensusService.sln"

SET ServiceName="ETSI Remote Consensus Service"
SET ServiceTargetPath="%BASE_PATH_TARGET%\RCService\"
SET ServiceOriginPath="%BASE_PATH_RC%\Etsi.RemoteConsensusServiceSetup\bin\Release\"

echo Step1 [RC]: Build RC application

echo Building RC solution
devenv %SOLUTION_PATH_RC% /clean Release | findstr /R /C:"===="
devenv %SOLUTION_PATH_RC% /build Release | findstr /R /C:"===="

echo Step2[RC]: Creating the services directory and moving the files in.
IF exist %BASE_PATH_TARGET% ( echo %BASE_PATH_TARGET% exists ) ELSE ( mkdir %BASE_PATH_TARGET% && echo %BASE_PATH_TARGET% created)

REM ----------------------------------------------------------------------------
REM                           RC Service
REM ----------------------------------------------------------------------------
echo Step 2.1[RC]: Installing RC Service.

REM Stop service
NET STOP %ServiceName%

REM Empty target folder (remove and recreate it)
RMDIR /S /Q %ServiceTargetPath%
mkdir %ServiceTargetPath%

REM Copy files that have been compiled to new folder
chdir /D %ServiceOriginPath% 
xcopy /S * %ServiceTargetPath%

REM Copy additional files (Currently there is an issue in solution to copy these dlls)
xcopy "%BASE_PATH_RC%\Capgemini.Etsi.RemoteConsensusService.Business\Lib\Microsoft.Practices.EnterpriseLibrary.Common.dll" "%ServiceTargetPath%" /b/v/y
xcopy "%BASE_PATH_RC%\Capgemini.Etsi.RemoteConsensusService.Business\Lib\Microsoft.Practices.ServiceLocation.dll" "%ServiceTargetPath%" /b/v/y
xcopy "%BASE_PATH_RC%\Capgemini.Etsi.RemoteConsensusService.Business\Lib\Microsoft.Practices.Unity.Interception.dll" "%ServiceTargetPath%" /b/v/y


REM Reinstall service
chdir /D %ServiceTargetPath%

REM Uninstalling service if it already existed
installutil.exe /U "Etsi.RemoteConsensusServiceSetup.exe"

REM Installing the service
installutil.exe "Etsi.RemoteConsensusServiceSetup.exe"

REM Configuring the service so that it starts automatically
sc config %ServiceName% start= auto

REM Start the service
NET START %ServiceName%

echo RC Service installed.
echo