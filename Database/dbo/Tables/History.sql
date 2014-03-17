CREATE TABLE [dbo].[History](
	[Pk_HistoryId] [int] IDENTITY(1,1) NOT NULL,
	[Fk_ReleaseId] [int] NULL,
	[Fk_PersonId] [int] NULL,
	[CreationDate] [datetime] NULL,
	[HistoryText] [nvarchar](255) NULL,
	[PersonName]  AS ([dbo].[getPersonName]([Fk_PersonId])),
    CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED ([Pk_HistoryId] ASC),
    CONSTRAINT [FK_History_Releases] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId])
);

