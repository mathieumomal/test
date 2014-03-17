CREATE TABLE [dbo].[Remarks](
	[Pk_RemarkId] [int] IDENTITY(1,1) NOT NULL,
	[Fk_ReleaseId] [int] NULL,
	[Fk_WorkItemId] [int] NULL,
	[Fk_PersonId] [int] NULL,
	[IsPublic] [bit] NULL,
	[CreationDate] [datetime] NULL,
	[RemarkText] [nvarchar](255) NULL,
	[PersonName]  AS ([dbo].[getPersonName]([Fk_PersonId])),
    CONSTRAINT [PK_Remarks] PRIMARY KEY CLUSTERED ([Pk_RemarkId] ASC),
    CONSTRAINT [FK_Remarks_Releases] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [FK_Remarks_WorkItems] FOREIGN KEY ([Fk_WorkItemId]) REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
);
GO

ALTER TABLE [dbo].[Remarks] ADD  CONSTRAINT [DF_Remarks_IsPublic]  DEFAULT ((1)) FOR [IsPublic]
GO