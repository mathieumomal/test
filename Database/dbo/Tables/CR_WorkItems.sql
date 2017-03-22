CREATE TABLE [dbo].[CR_WorkItems] (
    [Pk_CRWorkItems] INT IDENTITY (1, 1) NOT NULL,
    [Fk_CRId]        INT NULL,
    [Fk_WIId]        INT NULL,
    CONSTRAINT [PK_CR_WorkItems] PRIMARY KEY CLUSTERED ([Pk_CRWorkItems] ASC),
    CONSTRAINT [FK_CRId] FOREIGN KEY ([Fk_CRId]) REFERENCES [dbo].[ChangeRequest] ([Pk_ChangeRequest]),
    CONSTRAINT [FK_WIId] FOREIGN KEY ([Fk_WIId]) REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
);








GO
CREATE NONCLUSTERED INDEX [CrWorkitemIndexWithIncludedColumns]
    ON [dbo].[CR_WorkItems]([Fk_CRId] ASC)
    INCLUDE([Pk_CRWorkItems], [Fk_WIId]);


GO









-- =============================================
-- Author:		ETSI
-- Create date: 23/02/2017
-- Description:	TRIGGER for INSERT of CR WI. Set modification dates and db user which insert CRWI, 
-- inside both tables ChangeRequest and ChangeRequestTsgData
-- =============================================
CREATE TRIGGER [dbo].[TRIG_INSERT_CRWI]
   ON  [dbo].[CR_WorkItems]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @id int;
	select @id=i.Fk_CRId from inserted i;
	
	--CR
	UPDATE [dbo].ChangeRequest 
	SET MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
	WHERE Pk_ChangeRequest = @id;
	
	--CRTSGDATA (will update all the records inside ChangeRequestTsgData linked to the concerned ChangeRequest)
	UPDATE [dbo].ChangeRequestTsgData 
	SET MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
	WHERE Fk_ChangeRequest = @id;

    PRINT 'U3GPPDB.dbo.CR_WorkItems TRIGGER [INSERT]: TRIG_INSERT_CRWI - FIRED!'
END
GO









-- =============================================
-- Author:		ETSI
-- Create date: 23/02/2017
-- Description:	TRIGGER for DELETE of CR WI. Set modification dates and db user which delete CRWI, 
-- inside both tables ChangeRequest and ChangeRequestTsgData
-- =============================================
CREATE TRIGGER [dbo].[TRIG_DELETE_CRWI]
   ON  [dbo].[CR_WorkItems]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @id int;
	select @id=i.Fk_CRId from deleted i;
	
	--CR
	UPDATE [dbo].ChangeRequest 
	SET MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
	WHERE Pk_ChangeRequest = @id;
	
	--CRTSGDATA (will update all the records inside ChangeRequestTsgData linked to the concerned ChangeRequest)
	UPDATE [dbo].ChangeRequestTsgData 
	SET MOD_TS = GETDATE(), MOD_BY = (SELECT SYSTEM_USER)
	WHERE Fk_ChangeRequest = @id;

    PRINT 'U3GPPDB.dbo.CR_WorkItems TRIGGER [DELETE]: TRIG_DELETE_CRWI - FIRED!'
END