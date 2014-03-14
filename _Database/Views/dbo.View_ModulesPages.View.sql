USE [U3GPPDB]
GO

/****** Object:  View [dbo].[View_ModulesPages]    Script Date: 03/14/2014 09:33:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[View_ModulesPages] AS

SELECT t.TabName, t.TabPath, m.ModuleID, t.TabID

FROM DNN3GPP.dbo.Tabs t
INNER JOIN  DNN3GPP.dbo.TabModules tm ON t.TabID = tm.TabID
INNER JOIN  DNN3GPP.dbo.Modules m ON m.ModuleID = tm.ModuleID
INNER JOIN  DNN3GPP.dbo.ModuleDefinitions md ON md.ModuleDefID = m.ModuleDefID


GO


