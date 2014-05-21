-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Versions_CleanAll]
AS
BEGIN
	Declare @Error int

	DELETE FROM dbo.[Version];
	
	Select @Error = @@error
	Return @Error
END