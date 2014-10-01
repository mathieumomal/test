@ECHO OFF
SETLOCAL

REM -----------------------------------------------------------
REM ----- Launch DNN Setup for Online Machine -----------------
REM -----------------------------------------------------------

REM ------------------------------------------------------------------------------
REM ----------------  Provide Environment Parameters Below -----------------------
REM ------------------------------------------------------------------------------
SET PATH_ULTIMATE=D:\ETSI_Projects\etsi_ngpp_source_code\ULTIMATE\trunk
SET PATH_NGPP=D:\ETSI_Projects\etsi_ngpp_source_code\branches\Ultimate
SET BASE_PATH_TARGET=C:\EtsiPortalServices
SET SERVICE_LOGIN=CORP\aroucadi

REM ----------------------------------------------------------------------------------------
REM -----------------------  DON'T CHANGE ANYTHING BELOW------------------------------------
REM ----------------------------------------------------------------------------------------
SET SERVICE_USERRIGHTS="%PATH_ULTIMATE%\WCF Services\UserRightsService"
SET SERVICE_REMOTE_CONSENSUS=%PATH_NGPP%\RemoteConsensusService
SET SERVICE_VFS_HOST_FTP=%PATH_NGPP%\Contribution
SET SERVICE_OFFLINE_SYNC_SERVICE=%PATH_ULTIMATE%
SET SERVICE_ULTIMATE="%PATH_ULTIMATE%\WCF Services\UltimateService"
SET BATCHLOCATION="%~dp0"

REM -----------------------------------------------------------------
REM ------------------------- 01. SERVICES --------------------------
REM -----------------------------------------------------------------
ECHO @@@ Please enter your password (feel free to skip by enter) ?
set /P SERVICE_PASSWORD=
CLS
ECHO Service Installation Started..

CALL %BATCHLOCATION%\Setups\Online_Services.bat %SERVICE_USERRIGHTS% %SERVICE_REMOTE_CONSENSUS% %SERVICE_VFS_HOST_FTP% %SERVICE_OFFLINE_SYNC_SERVICE% %SERVICE_ULTIMATE% %BASE_PATH_TARGET% %SERVICE_LOGIN% %SERVICE_PASSWORD%
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
ECHO Local Environment Setup Done !!! SUCCESSFULLY !!!
GOTO END

:FAILED 
REM --------------------------------------------
REM ----------------  ERROR MESSAGE ------------
REM --------------------------------------------
ECHO Local Environment Setup !!! FAILED !!! \n
GOTO END

:END
ECHO -
ECHO (Local Environment Setup - END - %date% %time%)

PAUSE
ENDLOCAL
