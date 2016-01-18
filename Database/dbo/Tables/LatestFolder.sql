CREATE TABLE [dbo].[LatestFolder] (
    [Pk_LatestFolderId] INT            IDENTITY (1, 1) NOT NULL,
    [FolderName]        VARCHAR (50)   NOT NULL,
    [CreationDate]      DATE           NULL,
    [UserName]          NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_LatestFolder] PRIMARY KEY CLUSTERED ([Pk_LatestFolderId] ASC)
);

