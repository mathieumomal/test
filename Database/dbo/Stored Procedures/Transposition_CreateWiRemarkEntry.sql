-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transposition_CreateWiRemarkEntry]
	@WKI_ID integer,
	@SEQ_NO integer,
	@REMARK_TEXT varchar(255)
	
AS
BEGIN
	Declare @Error int

	DECLARE @NEW_WKI_REF varchar(30);
	DECLARE @NOW DATETIME;
	SELECT @NOW=GETDATE();
	exec [$(WPMDB)].[dbo].sp_ImportRemark @WKI_ID, @SEQ_NO, 'SECRETARIAT', @NOW, '3GPP Status', @REMARK_TEXT, '3GPP Status'
	
	Select @Error = @@error
	Return @Error
	
END