
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transposition_CreateEtsiWorkItem]
	@NEW_WKI_ID integer OUTPUT,
	@ETSI_NUMBER varchar(30),
	@STANDARD_TYPE varchar(2),
	@ETSI_DOC_NUMBER integer,
	@ETSI_PART_NUMBER integer,
	@REFERENCE varchar(30),
	@SERIAL_NUMBER varchar(20),
	@VERSION varchar(12),
	@COMMUNITY_ID integer,
	@TITLE_PART1 varchar (255),
	@TITLE_PART2 varchar (255),
	@TITLE_PART3 varchar (255),
	@RAPPORTEUR_ID integer,
	@SECRETARY_ID integer,
	@WORKING_TITLE varchar(255)
	
	
AS
BEGIN
	Declare @Error int

	DECLARE @NEW_WKI_REF varchar(30);
	DECLARE @WKI_ID integer;
	DECLARE @NOW DATETIME;
	DECLARE @INTWOMONTHS DATETIME;
	SELECT @NOW=GETDATE();
	SELECT @INTWOMONTHS = DATEADD(month,2,@NOW) 
	exec [WPMDB].[dbo].sp_ImportWorkItem 1, @ETSI_NUMBER, @STANDARD_TYPE, 1, @ETSI_DOC_NUMBER, @ETSI_PART_NUMBER, NULL, NULL, NULL, NULL, NULL,NULL,@REFERENCE,'R',@STANDARD_TYPE, @SERIAL_NUMBER,NULL,NULL,2,8,NULL,NULL,NULL,NULL,NULL,NULL,'PU',NULL,4,
			@VERSION,@COMMUNITY_ID,NULL,NULL,@TITLE_PART1,@TITLE_PART2,@TITLE_PART3,@RAPPORTEUR_ID,NULL,@SECRETARY_ID,NULL,'N',NULL,@NOW,NULL,NULL,'none','new',@NOW,0,0,0,'Y',@NOW,NULL,NULL,NULL,NULL,NULL,'N','N','N','N',
			'N','N','N',NULL,NULL,'3GPP Specs S',NULL,@INTWOMONTHS,@NOW,'3GPP Specs S',NULL,'N',NULL,NULL,NULL,NULL,NULL,NULL,@WORKING_TITLE,@WKI_ID OUTPUT,@NEW_WKI_REF OUTPUT
	
	SELECT @NEW_WKI_ID = @WKI_ID;
	
	Select @Error = @@error
	Return @Error
	
END