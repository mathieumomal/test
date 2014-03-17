
CREATE VIEW [dbo].[View_ModulesPages] 
AS
SELECT t.TabName, t.TabPath, m.ModuleID, t.TabID
FROM [$(DNN3GPP)].dbo.Tabs t
INNER JOIN  [$(DNN3GPP)].dbo.TabModules tm ON t.TabID = tm.TabID
INNER JOIN  [$(DNN3GPP)].dbo.Modules m ON m.ModuleID = tm.ModuleID
INNER JOIN  [$(DNN3GPP)].dbo.ModuleDefinitions md ON md.ModuleDefID = m.ModuleDefID

