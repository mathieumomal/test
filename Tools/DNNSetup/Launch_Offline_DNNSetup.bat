@ECHO OFF
SETLOCAL

REM ------------------------------------------------------------
REM ----- Launch DNN Setup for Offline Machine -----------------
REM ------------------------------------------------------------

REM ------------------------------------------------------------------------------
REM ----------------  Provide Environment Parameters Below -----------------------
REM ------------------------------------------------------------------------------
REM --- TEST IF SETTINGS FILE EXIST
ECHO Search for settings ...
SET mypath=%~dp0
SET settingsFile=%mypath%..\3GPP.sync.settings.bat
IF NOT EXIST "%settingsFile%" (
    ECHO "!!! WARNING !!! : Settings file doesn't exist.
	ECHO "Please create and customize .\3GPP.sync.settings.bat 
	ECHO "(You could find an example in Ultimate project > Tools > 3GPP.sync.settings.DEFAULT.bat)
	PAUSE
	EXIT
)
ECHO Settings found.
REM --- GET SETTINGS
CALL %settingsFile%

REM ----------------------------------------------------------------------------------------
REM -----------------------  DON'T CHANGE ANYTHING BELOW------------------------------------
REM ----------------------------------------------------------------------------------------
SET SERVICE_OFFLINE_SYNC_CLIENT=%PATH_ULTIMATE%
SET BATCHLOCATION="%~dp0"

REM -----------------------------------------------------------------
REM ------------------------- 01. SERVICES --------------------------
REM -----------------------------------------------------------------
ECHO @@@ Please enter your password (feel free to skip by enter) ?
set /P SERVICE_PASSWORD=
CLS
CALL %BATCHLOCATION%\Setups\Offline_Services.bat %SERVICE_OFFLINE_SYNC_CLIENT% %BASE_PATH_TARGET% %SERVICE_LOGIN% %SERVICE_PASSWORD%
IF %ERRORLEVEL% NEQ 0 GOTO FAILED

REM -----------------------------------------------------------------
REM ------------------------- 02. MODULES ---------------------------
REM -----------------------------------------------------------------

REM -----------------------------------------------------------------
REM ------------------------ 03. CSS STYLES -------------------------
REM -----------------------------------------------------------------


REM --------------------------------------------
REM ---------------  SUCCESS MESSAGE -----------
REM --------------------------------------------
ECHO Local Environment Setup Done Successfully
GOTO END

:FAILED 
REM --------------------------------------------
REM ----------------  ERROR MESSAGE ------------
REM --------------------------------------------
ECHO Local Environment Setup Failed...!!!
GOTO END

:END
ECHO Local Environment Setup - END - %date% %time%

PAUSE
ENDLOCAL
