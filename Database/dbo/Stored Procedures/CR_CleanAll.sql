-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CR_CleanAll]
AS
BEGIN
	Declare @Error int

	-- TRUNCATE TABLE CR
	TRUNCATE TABLE SyncInfo;
	TRUNCATE TABLE CR_WorkItems;
	DELETE FROM Remarks WHERE Fk_CRId IS NOT NULL;
	DELETE FROM ChangeRequest;
	DBCC CHECKIDENT('ChangeRequest', RESEED, 0)
	DELETE FROM Enum_CRCategory;
	DBCC CHECKIDENT('Enum_CRCategory', RESEED, 0)
	DELETE FROM Enum_CRImpact;
	DBCC CHECKIDENT('Enum_CRImpact', RESEED, 0)
	
	
	Select @Error = @@error
	Return @Error
	
END