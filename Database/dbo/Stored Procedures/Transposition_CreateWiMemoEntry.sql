
CREATE PROCEDURE [dbo].[Transposition_CreateWiMemoEntry]
	@WKI_ID integer,
	@WKI_SCOPE text

AS
BEGIN
	Declare @Error int

	exec [WPMDB].[dbo].sp_ImportMemo @WKI_ID, @WKI_SCOPE
	
	Select @Error = @@error
	Return @Error
	
END