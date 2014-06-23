CREATE TABLE [dbo].[History] (
    [Pk_HistoryId]       INT            IDENTITY (1, 1) NOT NULL,
    [Fk_ReleaseId]       INT            NULL,
    [Fk_PersonId]        INT            NULL,
    [CreationDate]       DATETIME       NULL,
    [HistoryText]        NVARCHAR (255) NULL,
    [PersonName]         AS             ([dbo].[getPersonName]([Fk_PersonId])),
    [Fk_SpecificationId] INT            NULL,
    [Fk_CRId]            INT            NULL,
    CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED ([Pk_HistoryId] ASC),
    CONSTRAINT [FK_History_CR] FOREIGN KEY ([Fk_CRId]) REFERENCES [dbo].[ChangeRequest] ([Pk_ChangeRequest]),
    CONSTRAINT [FK_History_Releases] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [FK_History_Specification] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);









