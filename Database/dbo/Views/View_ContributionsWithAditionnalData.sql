


CREATE VIEW [dbo].[View_ContributionsWithAditionnalData]
AS

SELECT		c.pk_Contribution
,			c.uid
,			ca.fk_SpecificationId
,			ca.Specification_MajorVersion
,			ca.Specification_TechnicalVersion
,			ca.Specification_EditorialVersion
FROM       [$(CONTRIB3GPPDB)].dbo.Contribution c
INNER JOIN [$(CONTRIB3GPPDB)].dbo.ContributionAdditionalData ca       ON ca.pk_ContributionAdditionalDatas = c.pk_Contribution
WHERE c.fk_Enum_ContributionType <> 
(SELECT ec.pk_Enum_ContributionType FROM [$(CONTRIB3GPPDB)].dbo.Enum_ContributionType as ec WHERE ec.Enum_Code = 'CR')
-- CR ARE NOT REQUIRED HERE BECAUSE WE ARE ON ULTIMATE SIDE SO WE ALREADY HAVE CR DATA