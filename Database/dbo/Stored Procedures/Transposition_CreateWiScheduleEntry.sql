-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transposition_CreateWiScheduleEntry]
	@WKI_ID integer,
	@SCHED_ID integer
	
AS
BEGIN
	Declare @Error int

	DECLARE @NOW DATETIME;
	SELECT @NOW=GETDATE();
	exec [$(WPMDB)].[dbo].sp_ImportSchedule @WKI_ID, @SCHED_ID, 0, '', '','',0,'','','N',@NOW,@NOW,@NOW,@NOW,'N',NULL,0,'N',0, 'N',0,'',0,0,0,''
	
	Select @Error = @@error
	Return @Error
	
END