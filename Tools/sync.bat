@ECHO OFF
REM ----- FILL IN THIS AREA -----
SET TRUNK_PATH=D:\CG_projects\Ultimate\
SET DNN_PATH=C:\websites\localdev.me\
SET PHASE=local

REM ----- DON'T MODIFY ANYTHING BELOW, unless you know what you're doing -----

REM Copying DLLs.
SET USERSOURCE=%TRUNK_PATH%bin\
SET USERDEST=%DNN_PATH%bin\
REM Copying Lib DLLs.
SET USERLIBSOURCE=%TRUNK_PATH%Libs\
xcopy "%USERLIBSOURCE%NetOffice.dll" "%USERDEST%" /b/v/y
xcopy "%USERLIBSOURCE%OfficeApi.dll" "%USERDEST%" /b/v/y
xcopy "%USERLIBSOURCE%VBIDEApi.dll" "%USERDEST%" /b/v/y
xcopy "%USERLIBSOURCE%WordApi.dll" "%USERDEST%" /b/v/y

xcopy "%USERSOURCE%Etsi.Ultimate.Business.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Controls.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.DataAccess.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.DomainClasses.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.CRs.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Meetings.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.MinuteMan.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Release.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Specifications.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Versions.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.WorkItem.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Repositories.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Services.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Utils.dll" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Utils.Core.dll" "%USERDEST%" /b/v/y

xcopy "%USERSOURCE%Etsi.Ultimate.Business.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Controls.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.DataAccess.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.DomainClasses.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.CRs.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Meetings.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.MinuteMan.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Release.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Specifications.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.Versions.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Module.WorkItem.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Repositories.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Services.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Utils.pdb" "%USERDEST%" /b/v/y
xcopy "%USERSOURCE%Etsi.Ultimate.Utils.Core.pdb" "%USERDEST%" /b/v/y

REM Copy configuration files
xcopy "%TRUNK_PATH%web.config.%PHASE%" "%DNN_PATH%web.config" /b/v/y

REM Release component
SET RELEASESOURCES=%TRUNK_PATH%Modules\Release\
SET RELEASEDEST=%DNN_PATH%DesktopModules\Release\
xcopy "%RELEASESOURCES%*.ascx" "%RELEASEDEST%" /b/v/y
xcopy "%RELEASESOURCES%*.aspx" "%RELEASEDEST%" /b/v/y
xcopy "%RELEASESOURCES%*.css" "%RELEASEDEST%" /b/v/y

SET aUIUSERSOURCE=%TRUNK_PATH%Modules\Controls\
SET aUIUSERDEST=%DNN_PATH%controls\Ultimate\
xcopy "%aUIUSERSOURCE%*.ascx" "%aUIUSERDEST%" /b/v/y
xcopy "%aUIUSERSOURCE%*.css" "%aUIUSERDEST%" /b/v/y
xcopy "%aUIUSERSOURCE%images\*" "%aUIUSERDEST%images\" /b/v/y


SET UIUSERSOURCE1=%TRUNK_PATH%Modules\WorkItem\
SET UIUSERDEST1=%DNN_PATH%DesktopModules\WorkItem\
xcopy "%UIUSERSOURCE1%*.ascx" "%UIUSERDEST1%" /b/v/y
xcopy "%UIUSERSOURCE1%*.aspx" "%UIUSERDEST1%" /b/v/y
xcopy "%UIUSERSOURCE1%*.css" "%UIUSERDEST1%" /b/v/y

SET SPECSOURCES=%TRUNK_PATH%Modules\Specifications\
SET SPECSOURCESDEST=%DNN_PATH%DesktopModules\Specifications\
xcopy "%SPECSOURCES%*.ascx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.aspx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.css" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%images\*" "%SPECSOURCESDEST%images\" /b/v/y
xcopy "%SPECSOURCES%JS\*" "%SPECSOURCESDEST%JS\" /b/v/y
xcopy "%SPECSOURCES%CSS\*" "%SPECSOURCESDEST%CSS\" /b/v/y

SET SPECSOURCES=%TRUNK_PATH%Modules\Versions\
SET SPECSOURCESDEST=%DNN_PATH%DesktopModules\Versions\
xcopy "%SPECSOURCES%*.ascx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.aspx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.css" "%SPECSOURCESDEST%" /b/v/y

SET SPECSOURCES=%TRUNK_PATH%Modules\CRs\
SET SPECSOURCESDEST=%DNN_PATH%DesktopModules\CRs\
xcopy "%SPECSOURCES%*.ascx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.aspx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.css" "%SPECSOURCESDEST%" /b/v/y

SET SPECSOURCES=%TRUNK_PATH%Modules\Meetings\
SET SPECSOURCESDEST=%DNN_PATH%DesktopModules\Meetings\
xcopy "%SPECSOURCES%*.ascx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.aspx" "%SPECSOURCESDEST%" /b/v/y
xcopy "%SPECSOURCES%*.css" "%SPECSOURCESDEST%" /b/v/y