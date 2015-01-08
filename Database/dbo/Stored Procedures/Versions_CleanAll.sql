-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Versions_CleanAll]
AS
BEGIN
	Declare @Error int

	DELETE FROM dbo.Remarks WHERE Fk_VersionId IS NOT NULL
	DELETE FROM dbo.[Version];
	DBCC CHECKIDENT('Version', RESEED, 0)
	
	
	Select @Error = @@error
	Return @Error
END