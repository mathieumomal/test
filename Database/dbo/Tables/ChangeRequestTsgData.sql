CREATE TABLE [dbo].[ChangeRequestTsgData] (
    [Pk_ChangeRequestTsgData] INT            IDENTITY (1, 1) NOT NULL,
    [TSGTdoc]                 VARCHAR (200)  NULL,
    [TSGTarget]               INT            NULL,
    [TSGMeeting]              INT            NULL,
    [TSGSourceOrganizations]  VARCHAR (1000) NULL,
    [Fk_ChangeRequest]        INT            NOT NULL,
    [Fk_TsgStatus]            INT            NULL,
    [CreationDate]            DATETIME       NULL,
    [MOD_BY]                  VARCHAR (100)  NULL,
    [MOD_TS]                  DATETIME       NULL,
    CONSTRAINT [PK_ChangeRequestTsgData] PRIMARY KEY CLUSTERED ([Pk_ChangeRequestTsgData] ASC),
    CONSTRAINT [FK_ChangeRequestTsgData_ChangeRequest] FOREIGN KEY ([Fk_ChangeRequest]) REFERENCES [dbo].[ChangeRequest] ([Pk_ChangeRequest]),
    CONSTRAINT [FK_ChangeRequestTsgData_TsgStatus] FOREIGN KEY ([Fk_TsgStatus]) REFERENCES [dbo].[Enum_ChangeRequestStatus] ([Pk_EnumChangeRequestStatus])
);


















GO
CREATE NONCLUSTERED INDEX [TsgTDocUid_index]
    ON [dbo].[ChangeRequestTsgData]([TSGTdoc] ASC);


GO
CREATE NONCLUSTERED INDEX [TsgTdocIndex]
    ON [dbo].[ChangeRequestTsgData]([TSGTdoc] ASC);


GO
CREATE NONCLUSTERED INDEX [CRTsgDataIndexWithIncludedColumns]
    ON [dbo].[ChangeRequestTsgData]([Fk_ChangeRequest] ASC)
    INCLUDE([Pk_ChangeRequestTsgData], [TSGTdoc], [TSGTarget], [TSGMeeting], [TSGSourceOrganizations], [Fk_TsgStatus]);


GO







-- =============================================
-- Author:		ETSI
-- Create date: 03/02/2017
-- Description:	TRIGGER for UPDATE of CR TSG data. Set modification dates and db user which insert CR
-- =============================================
CREATE TRIGGER [dbo].[TRIG_UPDATE_CRTSG]
   ON  [dbo].[ChangeRequestTsgData]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @id int;
	select @id=i.Pk_ChangeRequestTsgData from inserted i;
	
	IF(TRIGGER_NESTLEVEL() <= 1)
	BEGIN
		UPDATE [dbo].ChangeRequestTsgData 
		SET MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
		WHERE Pk_ChangeRequestTsgData = @id;
		PRINT 'U3GPPDB.dbo.ChangeRequestTsgData TRIGGER [UPDATE]: TRIG_UPDATE_CRTSG - FIRED!'
	END
END
GO








-- =============================================
-- Author:		ETSI
-- Create date: 03/02/2017
-- Description:	TRIGGER for INSERT of CR TSG data. Set creation date
-- =============================================
CREATE TRIGGER [dbo].[TRIG_INSERT_CRTSG]
   ON  [dbo].[ChangeRequestTsgData]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @id int;
	select @id=i.Pk_ChangeRequestTsgData from inserted i;
	
	IF(TRIGGER_NESTLEVEL() <= 1)
	BEGIN
		UPDATE [dbo].ChangeRequestTsgData 
		SET CreationDate = GETDATE(), MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
		WHERE Pk_ChangeRequestTsgData = @id;
		PRINT 'U3GPPDB.dbo.ChangeRequestTsgData TRIGGER [INSERT]: TRIG_INSERT_CRTSG - FIRED!'
	END
END