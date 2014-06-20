CREATE TABLE [dbo].[Remarks] (
    [Pk_RemarkId]               INT            IDENTITY (1, 1) NOT NULL,
    [Fk_ReleaseId]              INT            NULL,
    [Fk_WorkItemId]             INT            NULL,
    [Fk_PersonId]               INT            NULL,
    [IsPublic]                  BIT            CONSTRAINT [DF_Remarks_IsPublic] DEFAULT ((1)) NULL,
    [CreationDate]              DATETIME       NULL,
    [RemarkText]                NVARCHAR (255) NULL,
    [PersonName]                AS             ([dbo].[getPersonName]([Fk_PersonId])),
    [Fk_SpecificationId]        INT            NULL,
    [Fk_SpecificationReleaseId] INT            NULL,
    [Fk_VersionId]              INT            NULL,
    [Fk_CRId]                   INT            NULL,
    CONSTRAINT [PK_Remarks] PRIMARY KEY CLUSTERED ([Pk_RemarkId] ASC),
    CONSTRAINT [FK_Remarks_CR] FOREIGN KEY ([Fk_CRId]) REFERENCES [dbo].[ChangeRequest] ([Pk_ChangeRequest]),
    CONSTRAINT [FK_Remarks_Releases] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [FK_Remarks_Specification] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId]),
    CONSTRAINT [FK_Remarks_Specification_Release] FOREIGN KEY ([Fk_SpecificationReleaseId]) REFERENCES [dbo].[Specification_Release] ([Pk_Specification_ReleaseId]),
    CONSTRAINT [FK_Remarks_Version] FOREIGN KEY ([Fk_VersionId]) REFERENCES [dbo].[Version] ([Pk_VersionId]),
    CONSTRAINT [FK_Remarks_WorkItems] FOREIGN KEY ([Fk_WorkItemId]) REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
);






GO


GO