@ECHO OFF
SETLOCAL

REM ------------------------------------------------------------
REM ----- Launch DNN Setup for Offline Machine -----------------
REM ------------------------------------------------------------

REM ------------------------------------------------------------------------------
REM ----------------  Provide Environment Parameters Below -----------------------
REM ------------------------------------------------------------------------------
SET PATH_ULTIMATE=D:\CG_projects\Ultimate
SET BASE_PATH_TARGET=C:\EtsiPortalServices
SET SERVICE_LOGIN=CORP\[your login]

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
