CREATE TABLE [dbo].[ChangeRequest] (
    [Pk_ChangeRequest]       INT           NOT NULL,
    [CRNumber]               VARCHAR (10)  NOT NULL,
    [Revision]               INT           NULL,
    [Subject]                VARCHAR (300) NULL,
    [Fk_TSGStatus]           INT           NULL,
    [Fk_WGStatus]            INT           NULL,
    [CreationDate]           DATETIME      NULL,
    [TSGSourceOrganizations] VARCHAR (100) NULL,
    [WGSourceOrganizations]  VARCHAR (100) NULL,
    [TSGMeeting]             INT           NULL,
    [TSGTarget]              INT           NULL,
    [WGSourceForTSG]         INT           NULL,
    [Fk_TSGTDoc]             INT           NULL,
    [WGMeeting]              INT           NULL,
    [WGTarget]               INT           NULL,
    [Fk_WGTDoc]              INT           NULL,
    [Fk_Enum_CRCategory]     INT           NULL,
    [Fk_SpecRelease]         INT           NOT NULL,
    [Fk_Versions]            INT           NOT NULL,
    CONSTRAINT [PK_ChangeRequest] PRIMARY KEY CLUSTERED ([Pk_ChangeRequest] ASC),
    CONSTRAINT [FK_CR_Version] FOREIGN KEY ([Fk_Versions]) REFERENCES [dbo].[CR_Version] ([Pk_SpecVersion]),
    CONSTRAINT [FK_Enum_CRCategory] FOREIGN KEY ([Fk_Enum_CRCategory]) REFERENCES [dbo].[Enum_CRCategory] ([Pk_EnumCRCategory]),
    CONSTRAINT [FK_SpecRelease] FOREIGN KEY ([Fk_SpecRelease]) REFERENCES [dbo].[Specification_Release] ([Pk_Specification_ReleaseId]),
    CONSTRAINT [FK_TSGStatus] FOREIGN KEY ([Fk_TSGStatus]) REFERENCES [dbo].[Enum_TDocStatus] ([Pk_EnumTDocStatus]),
    CONSTRAINT [FK_WGStatus] FOREIGN KEY ([Fk_WGStatus]) REFERENCES [dbo].[Enum_TDocStatus] ([Pk_EnumTDocStatus])
);
















GO
CREATE NONCLUSTERED INDEX [IX_ChangeRequest]
    ON [dbo].[ChangeRequest]([Pk_ChangeRequest] ASC);

