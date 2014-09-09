@ECHO OFF
SETLOCAL

REM -----------------------------------------------------------
REM ----- Launch DNN Setup for Online Machine -----------------
REM -----------------------------------------------------------

REM ------------------------------------------------------------------------------
REM ----------------  Provide Environment Parameters Below -----------------------
REM ------------------------------------------------------------------------------
SET PATH_ULTIMATE=D:\3GPP\SourceCode\ULTIMATE\trunk
SET PATH_NGPP=D:\3GPP\SourceCode\NGPP
SET BASE_PATH_TARGET=C:\EtsiPortalServices
SET SERVICE_LOGIN=CORP\cneelam
SET SERVICE_PASSWORD=SECRET

REM ----------------------------------------------------------------------------------------
REM -----------------------  DON'T CHANGE ANYTHING BELOW------------------------------------
REM ----------------------------------------------------------------------------------------
SET SERVICE_USERRIGHTS="%PATH_ULTIMATE%\WCF Services\UserRightsService"
SET SERVICE_REMOTE_CONSENSUS=%PATH_NGPP%\RemoteConsensusService
SET SERVICE_VFS_HOST_FTP=%PATH_NGPP%\Contribution
SET SERVICE_OFFLINE_SYNC_SERVICE=%PATH_ULTIMATE%
SET BATCHLOCATION="%~dp0"

REM -----------------------------------------------------------------
REM ------------------------- 01. SERVICES --------------------------
REM -----------------------------------------------------------------
ECHO Service Installation Started..
CALL %BATCHLOCATION%\Setups\Online_Services.bat %SERVICE_USERRIGHTS% %SERVICE_REMOTE_CONSENSUS% %SERVICE_VFS_HOST_FTP% %SERVICE_OFFLINE_SYNC_SERVICE% %BASE_PATH_TARGET% %SERVICE_LOGIN% %SERVICE_PASSWORD%
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
