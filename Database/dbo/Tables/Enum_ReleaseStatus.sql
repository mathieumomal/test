CREATE TABLE [dbo].[Enum_ReleaseStatus] (
    [Enum_ReleaseStatusId] INT          NOT NULL,
    [Code]                 VARCHAR (10) NULL,
    [Description]          VARCHAR (50) NULL,
    CONSTRAINT [PK_Enum_ReleaseStatus] PRIMARY KEY CLUSTERED ([Enum_ReleaseStatusId] ASC),
    CONSTRAINT [IX_Enum_ReleaseStatus] UNIQUE NONCLUSTERED ([Enum_ReleaseStatusId] ASC)
);



