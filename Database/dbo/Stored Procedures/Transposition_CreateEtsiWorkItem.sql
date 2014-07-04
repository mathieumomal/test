-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transposition_CreateEtsiWorkItem]
	@TITLE_PART3 varchar (255),
	@NEW_WKI_ID integer OUTPUT
AS
BEGIN
	Declare @Error int

	DECLARE @NEW_WKI_REF varchar(30);
	DECLARE @NOW DATETIME;
	SELECT @NOW=GETDATE();
	exec [$(WPMDB)].[dbo].sp_ImportWorkItem 1, '', '', 0, 0, 0, 0, 0, 0,NULL, 0,'','REF-','R','','','','',0,0,'','','',0,'',0,'',0,0,
			0,0,'','','','',@TITLE_PART3,0,0,0,0,'N',NULL,@NOW,NULL,@NOW,'','',@NOW,0,0,0,'Y',@NOW,'','','','','','N','N','N','N',
			'N','N','N','','','','N',@NOW,@NOW,'','','N','',0,0,'',@NOW,'','',@NEW_WKI_ID,@NEW_WKI_REF
	
	Select @Error = @@error
	Return @Error
	
END