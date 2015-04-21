﻿/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\Enum_ChangeRequestStatus.data.sql
:r .\Enum_CommunitiesShortNames.data.sql
:r .\Enum_ReleaseStatus.data.sql
:r .\Update_CR_Revisions_0_To_NULL.sql
:r .\UpdateChangeRequestsData.sql
:r .\UpdateReleaseMtgShortReferences.sql