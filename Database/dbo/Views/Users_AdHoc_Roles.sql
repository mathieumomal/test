CREATE VIEW [dbo].[Users_AdHoc_Roles]
AS
SELECT up.UserID, up.PropertyValue as PERSON_ID, r.RoleName
FROM [$(DNN3GPP)].dbo.UserProfile up
INNER JOIN [$(DNN3GPP)].dbo.ProfilePropertyDefinition ppd ON ppd.PropertyDefinitionID = up.PropertyDefinitionID
INNER JOIN [$(DNN3GPP)].dbo.UserRoles ur ON ur.UserID = up.UserID
INNER JOIN [$(DNN3GPP)].dbo.Roles r ON r.RoleID = ur.RoleID

WHERE ppd.PropertyName = 'ETSI_DS_ID' 
AND r.RoleName IN ('Work Plan Managers', 'Specification Managers', 'Administrators')