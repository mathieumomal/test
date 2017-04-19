CREATE TABLE [dbo].[ChangeRequest] (
    [Pk_ChangeRequest]        INT            IDENTITY (1, 1) NOT NULL,
    [CRNumber]                VARCHAR (10)   NULL,
    [Revision]                INT            NULL,
    [Subject]                 VARCHAR (1000) NULL,
    [Fk_WGStatus]             INT            NULL,
    [CreationDate]            DATETIME       NULL,
    [WGSourceOrganizations]   VARCHAR (1000) NULL,
    [WGSourceForTSG]          INT            NULL,
    [WGMeeting]               INT            NULL,
    [WGTarget]                INT            NULL,
    [WGTDoc]                  VARCHAR (200)  NULL,
    [Fk_Enum_CRCategory]      INT            NULL,
    [Fk_Specification]        INT            NULL,
    [Fk_Release]              INT            NULL,
    [Fk_CurrentVersion]       INT            NULL,
    [Fk_NewVersion]           INT            NULL,
    [Fk_Impact]               INT            NULL,
    [MOD_BY]                  VARCHAR (100)  NULL,
    [MOD_TS]                  DATETIME       NULL,
    [ClausesAffected]         VARCHAR (500)  NULL,
    [OtherCoreSpecifications] VARCHAR (500)  NULL,
    [TestSpecifications]      VARCHAR (500)  NULL,
    [OMSpecifications]        VARCHAR (500)  NULL,
    CONSTRAINT [PK_ChangeRequest] PRIMARY KEY CLUSTERED ([Pk_ChangeRequest] ASC),
    CONSTRAINT [FK_ChangeRequest_WgStatus] FOREIGN KEY ([Fk_WGStatus]) REFERENCES [dbo].[Enum_ChangeRequestStatus] ([Pk_EnumChangeRequestStatus]),
    CONSTRAINT [FK_CRCurrentVersion] FOREIGN KEY ([Fk_CurrentVersion]) REFERENCES [dbo].[Version] ([Pk_VersionId]),
    CONSTRAINT [FK_CRNewVersion] FOREIGN KEY ([Fk_NewVersion]) REFERENCES [dbo].[Version] ([Pk_VersionId]),
    CONSTRAINT [FK_CRRelease] FOREIGN KEY ([Fk_Release]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [FK_CRSpecification] FOREIGN KEY ([Fk_Specification]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId]),
    CONSTRAINT [FK_Enum_CRCategory] FOREIGN KEY ([Fk_Enum_CRCategory]) REFERENCES [dbo].[Enum_CRCategory] ([Pk_EnumCRCategory]),
    CONSTRAINT [FK_Enum_CRImpact] FOREIGN KEY ([Fk_Impact]) REFERENCES [dbo].[Enum_CRImpact] ([Pk_EnumCRImpact])
);












































GO
CREATE NONCLUSTERED INDEX [IX_ChangeRequest]
    ON [dbo].[ChangeRequest]([Pk_ChangeRequest] ASC);


GO
CREATE NONCLUSTERED INDEX [WgTdocIndex]
    ON [dbo].[ChangeRequest]([WGTDoc] ASC);


GO









-- =============================================
-- Author:		ETSI
-- Create date: 03/02/2017
-- Description:	TRIGGER for UPDATE of CR. Set modification dates and db user which insert CR
-- =============================================
CREATE TRIGGER [dbo].[TRIG_UPDATE_CR]
   ON  [dbo].[ChangeRequest]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @id int;
	select @id=i.Pk_ChangeRequest from inserted i;
	
	IF(TRIGGER_NESTLEVEL() <= 1)
	BEGIN
		UPDATE [dbo].ChangeRequest 
		SET MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
		WHERE Pk_ChangeRequest = @id;
		PRINT 'U3GPPDB.dbo.ChangeRequest TRIGGER [UPDATE]: TRIG_UPDATE_CR - FIRED!'
	END
END
GO









-- =============================================
-- Author:		ETSI
-- Create date: 03/02/2017
-- Description:	TRIGGER for INSERT of CR. Set creation date
-- =============================================
CREATE TRIGGER [dbo].[TRIG_INSERT_CR]
   ON  [dbo].[ChangeRequest]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @id int;
	select @id=i.Pk_ChangeRequest from inserted i;
	
	IF(TRIGGER_NESTLEVEL() <= 1)
	BEGIN
		UPDATE [dbo].ChangeRequest 
		SET CreationDate = GETDATE(), MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
		WHERE Pk_ChangeRequest = @id;
		PRINT 'U3GPPDB.dbo.ChangeRequest TRIGGER [INSERT]: TRIG_INSERT_CR - FIRED!'
	END
END