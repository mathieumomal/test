USE [U3GPPDB]
GO
/****** Object:  View [dbo].[Users_AdHoc_Roles]    Script Date: 02/26/2014 14:33:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Users_AdHoc_Roles]
AS
SELECT up.UserID, up.PropertyValue as PERSON_ID, r.RoleName
FROM DNN3GPP..UserProfile up
INNER JOIN DNN3GPP..ProfilePropertyDefinition ppd ON ppd.PropertyDefinitionID = up.PropertyDefinitionID
INNER JOIN DNN3GPP..UserRoles ur ON ur.UserID = up.UserID
INNER JOIN DNN3GPP..Roles r ON r.RoleID = ur.RoleID

WHERE ppd.PropertyName = 'ETSI_DS_ID' 
AND r.RoleName IN ('Work Plan Managers', 'Specification Managers', 'Administrators')
GO
