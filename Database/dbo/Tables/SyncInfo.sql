CREATE TABLE [dbo].[SyncInfo] (
    [Pk_SyncId]     INT           IDENTITY (1, 1) NOT NULL,
    [TerminalName]  VARCHAR (250) NOT NULL,
    [Offline_PK_Id] INT           NOT NULL,
    [Fk_VersionId]  INT           NULL,
    CONSTRAINT [Pk_SyncInfo] PRIMARY KEY CLUSTERED ([Pk_SyncId] ASC),
    CONSTRAINT [FK_SyncInfo_Version] FOREIGN KEY ([Fk_VersionId]) REFERENCES [dbo].[Version] ([Pk_VersionId])
);

