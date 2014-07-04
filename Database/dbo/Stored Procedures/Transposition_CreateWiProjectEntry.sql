-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transposition_CreateWiProjectEntry]
	@WKI_ID integer,
	@PROJECT_ID integer

AS
BEGIN
	Declare @Error int

	DECLARE @NOW DATETIME;
	SELECT @NOW=GETDATE();
	exec [$(WPMDB)].[dbo].sp_ImportProject @WKI_ID, @PROJECT_ID, '3GPP Spec S'
	
	Select @Error = @@error
	Return @Error
	
END