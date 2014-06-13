-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE CR_CleanAll
AS
BEGIN
	Declare @Error int

	-- Delete from CR
	DELETE FROM Enum_CRCategory;
	
	Select @Error = @@error
	Return @Error
	
END