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
    [Fk_CR_Specification]    INT           NULL,
    [Fk_TargetVersion]       INT           NULL,
    [Fk_NewVersion]          INT           NULL,
    [Fk_TargetRelease]       INT           NULL,
    [TSGMeeting]             INT           NULL,
    [TSGTarget]              INT           NULL,
    [WGSourceForTSG]         INT           NULL,
    [Fk_TSGTDoc]             INT           NULL,
    [WGMeeting]              INT           NULL,
    [WGTarget]               INT           NULL,
    [Fk_WGTDoc]              INT           NULL,
    [Fk_Enum_CRCategory]     INT           NULL,
    CONSTRAINT [PK_ChangeRequest] PRIMARY KEY CLUSTERED ([Pk_ChangeRequest] ASC),
    CONSTRAINT [FK_CR_Specification] FOREIGN KEY ([Fk_CR_Specification]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId]),
    CONSTRAINT [FK_Enum_CRCategory] FOREIGN KEY ([Fk_Enum_CRCategory]) REFERENCES [dbo].[Enum_CRCategory] ([Pk_EnumCRCategory]),
    CONSTRAINT [FK_Release] FOREIGN KEY ([Fk_TargetRelease]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [FK_TSGStatus] FOREIGN KEY ([Fk_TSGStatus]) REFERENCES [dbo].[Enum_TDocStatus] ([Pk_EnumTDocStatus]),
    CONSTRAINT [FK_WGStatus] FOREIGN KEY ([Fk_WGStatus]) REFERENCES [dbo].[Enum_TDocStatus] ([Pk_EnumTDocStatus])
);












GO
CREATE NONCLUSTERED INDEX [IX_ChangeRequest]
    ON [dbo].[ChangeRequest]([Pk_ChangeRequest] ASC);

