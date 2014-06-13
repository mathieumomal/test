-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CR_CleanAll]
AS
BEGIN
	Declare @Error int

	-- Delete from CR
	DELETE FROM Enum_CRCategory;
	DELETE FROM Enum_TDocStatus;
	
	Select @Error = @@error
	Return @Error
	
END