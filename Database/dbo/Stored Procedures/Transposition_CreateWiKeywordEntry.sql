-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transposition_CreateWiKeywordEntry]
	@WKI_ID integer,
	@KEYWORD_CODE varchar(30)

AS
BEGIN
	Declare @Error int

	DECLARE @NOW DATETIME;
	SELECT @NOW=GETDATE();
	exec [$(WPMDB)].[dbo].sp_ImportKeyword @KEYWORD_CODE, @WKI_ID, '3GPP Spec S'
	
	Select @Error = @@error
	Return @Error
	
END