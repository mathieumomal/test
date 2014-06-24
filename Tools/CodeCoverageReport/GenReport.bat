@ECHO OFF
REM 
SET NUNIT=C:\Program Files (x86)\NUnit 2.6.3\bin
SET REPORT_GEN=ReportGenerator_1.9.1.0\bin
set OPEN_COVER=C:\Program Files (x86)\OpenCover
SET TRUNK_PATH=D:\ETSI_Projects\etsi_ngpp_source_code\ULTIMATE\trunk\
set TEST_PROJECT=%TRUNK_PATH%\Common\Tests\bin\Release
set OUTPUT=%TRUNK_PATH%\Tools\CodeCoverageReport
SET FILTER_EXPRESSION=+[Etsi.Ultimate.*]* -[Etsi.Ultimate.Tests]* -[Etsi.Ultimate.DomainClasses]* -[Etsi.Ultimate.DataAccess]* -[Etsi.Ultimate.Services]Etsi.Ultimate.Services.*Mock* -[Etsi.Ultimate.Utils]Etsi.Ultimate.Utils.WcfMailService.*

REM ----- DON'T MODIFY ANYTHING BELOW, unless you know what you're doing -----



"%OPEN_COVER%\OpenCover.Console.exe" -target:"%NUNIT%\nunit-console.exe" -targetargs:"/nologo  /noshadow %TEST_PROJECT%\Etsi.Ultimate.Tests.dll" -filter:"%FILTER_EXPRESSION%" -register:user  -output:"%OUTPUT%\XML\projectCoverageReport.xml"

REM Code coverage report generation
%REPORT_GEN%\ReportGenerator.exe -reports:"%OUTPUT%\XML\projectCoverageReport.xml" -targetdir:"%OUTPUT%\Report" 

