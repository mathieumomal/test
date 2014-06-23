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
	DELETE FROM CR_WorkItems;
	DELETE FROM Remarks;
	DELETE FROM Enum_CRCategory;
	DELETE FROM Enum_TDocStatus;
	DELETE FROM Enum_CRImpact;
	DELETE FROM ChangeRequest;
	
	Select @Error = @@error
	Return @Error
	
END