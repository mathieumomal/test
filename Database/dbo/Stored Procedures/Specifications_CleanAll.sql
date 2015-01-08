-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Specifications_CleanAll] 
	
AS
BEGIN
	Declare @Error int

	-- Delete from specifications
	DELETE FROM Remarks WHERE Fk_SpecificationId IS NOT NULL OR Fk_SpecificationReleaseId IS NOT NULL;
	DELETE FROM History WHERE Fk_SpecificationId IS NOT NULL;
	DELETE FROM SpecificationResponsibleGroup;
	DBCC CHECKIDENT('SpecificationResponsibleGroup', RESEED, 0)
	DELETE FROM SpecifcationsGenealogy;
	-- No ID KEY. DBCC CHECKIDENT('SpecifcationsGenealogy', RESEED, 0)
	DELETE FROM SpecificationTechnologies;
	DBCC CHECKIDENT('SpecificationTechnologies', RESEED, 0)
	DELETE FROM SpecificationRapporteur;
	DBCC CHECKIDENT('SpecificationRapporteur', RESEED, 0)
	DELETE FROM Specification_WorkItem;
	DBCC CHECKIDENT('Specification_WorkItem', RESEED, 0)
	DELETE FROM Specification_Release;
	DBCC CHECKIDENT('Specification_Release', RESEED, 0)
	DELETE FROM Specification;
	DBCC CHECKIDENT('Specification', RESEED, 0)
	DELETE FROM Enum_Serie;
	DBCC CHECKIDENT('Enum_Serie', RESEED, 0)
	DELETE FROM Enum_Technology;
	DBCC CHECKIDENT('Enum_Technology', RESEED, 0)
	
	Select @Error = @@error
	Return @Error
	
END