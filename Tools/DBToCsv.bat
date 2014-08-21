@ECHO OFF
REM ----------------------------------------------
REM Set Parameters Below
REM ----------------------------------------------

SET Location=D:\Users\cneelam\Desktop\
SET Server=(local)
SET Database=NGPPDB
SET Query=select top 10 * from dbo.Contribution


REM ----------------------------------------------
REM Don't Modify Anything Below
REM ----------------------------------------------

SET tempFile=%Location%Temp.csv
SET fileName=%Location%Export.csv

IF EXIST %tempFile% DEL %tempFile%
IF EXIST %fileName% DEL %fileName%
sqlcmd -S %Server% -d %Database% -E -s "," -W -Q "SET NOCOUNT ON;%Query%" | findstr /V /C:"-" /B > %tempFile%
SETLOCAL enableDelayedExpansion
set originalText=NULL
set replacedText=

for /f "tokens=*" %%a in ('type %tempFile%') do (
    set "line=%%a"
    if defined line (
        call set "line=%%line:%originalText%=%replacedText%%%"
        echo !line!>> %fileName%
    ) else (
        echo.
    )
)

IF EXIST %tempFile% DEL %tempFile%

