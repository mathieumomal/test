-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Specifications_CleanAll] 
	
AS
BEGIN


	-- Delete from specifications
	DELETE FROM Remarks;
	DELETE FROM SpecificationResponsibleGroup;
	DELETE FROM SpecifcationsGenealogy;
	DELETE FROM SpecificationTechnologies;
	DELETE FROM SpecificationRapporteur;
	DELETE FROM Specification_WorkItem;
	DELETE FROM Specification_Release;
	DELETE FROM Specification;
	DELETE FROM Enum_Serie;
	DELETE FROM Enum_Technology;
	
	
END