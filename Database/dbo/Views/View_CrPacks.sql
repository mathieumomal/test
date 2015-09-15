CREATE VIEW dbo.View_CrPacks
AS

SELECT     c.pk_Contribution
,          c.title
,          c.[uid]
,          ca.fk_Meeting
,          c.Denorm_Source 
,          mt.TB_ID
FROM       [$(CONTRIB3GPPDB)].dbo.Contribution c 
INNER JOIN [$(CONTRIB3GPPDB)].dbo.ContribAllocation ca       ON ca.fk_Contribution = c.pk_Contribution
INNER JOIN [$(CONTRIB3GPPDB)].dbo.Meetings_TB mt             ON mt.MTG_ID = ca.fk_Meeting
INNER JOIN [$(CONTRIB3GPPDB)].dbo.Enum_ContributionType ct   ON ct.pk_Enum_ContributionType = c.fk_Enum_ContributionType AND ct.Enum_Code = 'CRP'
INNER JOIN [$(CONTRIB3GPPDB)].dbo.Enum_ContributionStatus cs ON cs.pk_Enum_ContributionStatus = c.fk_Enum_ContributionStatus AND cs.Enum_Code = 'Reserved'

GO
