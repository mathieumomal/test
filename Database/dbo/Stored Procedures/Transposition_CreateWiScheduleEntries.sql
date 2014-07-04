-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transposition_CreateWiScheduleEntries]
	@WKI_ID integer,
	@MAJOR_VERSION integer,
	@TECHNICAL_VERSION integer,
	@EDITORIAL_VERSION integer
	
AS
BEGIN
	Declare @Error int

	DECLARE @NOW DATETIME;
	SELECT @NOW=GETDATE();
	exec [WPMDB].[dbo].sp_ImportSchedule @WKI_ID, 1, 1, 'TB', 'M40','Creation of WI by WG/TB',0,'','','N',NULL,NULL,NULL,NULL,'N',NULL,NULL,'N',10, 'N',0,NULL,NULL,NULL,NULL,'3GPP Status'
	exec [WPMDB].[dbo].sp_ImportSchedule @WKI_ID, 2, 2, 'TB', 'M28','TB approval',8,'','','N',NULL,NULL,NULL,NULL,'N',NULL,NULL,'N',10, 'Y',3,NULL,@MAJOR_VERSION,@TECHNICAL_VERSION,@EDITORIAL_VERSION,'3GPP Status'
	exec [WPMDB].[dbo].sp_ImportSchedule @WKI_ID, 3, 3, 'TB', 'M3','Draft receipt by ETSI Secretariat',8,'A','','N',NULL,NULL,NULL,NULL,'N',NULL,NULL,'N',10, 'Y',2,NULL,NULL,NULL,NULL,'3GPP Status'
	exec [WPMDB].[dbo].sp_ImportSchedule @WKI_ID, 4, 4, 'TB', 'M16','Publication',12,'','','N',NULL,NULL,NULL,NULL,'N',NULL,NULL,'N',10, 'Y',0,NULL,NULL,NULL,NULL,'3GPP Status'
	
	Select @Error = @@error
	Return @Error
	
END