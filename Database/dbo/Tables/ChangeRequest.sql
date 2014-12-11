CREATE TABLE [dbo].[ChangeRequest] (
    [Pk_ChangeRequest]       INT            IDENTITY (1, 1) NOT NULL,
    [CRNumber]               VARCHAR (10)   NULL,
    [Revision]               INT            NULL,
    [Subject]                VARCHAR (1000) NULL,
    [Fk_TSGStatus]           INT            NULL,
    [Fk_WGStatus]            INT            NULL,
    [CreationDate]           DATETIME       NULL,
    [TSGSourceOrganizations] VARCHAR (1000)  NULL,
    [WGSourceOrganizations]  VARCHAR (1000)  NULL,
    [TSGMeeting]             INT            NULL,
    [TSGTarget]              INT            NULL,
    [WGSourceForTSG]         INT            NULL,
    [TSGTDoc]                VARCHAR (50)   NULL,
    [WGMeeting]              INT            NULL,
    [WGTarget]               INT            NULL,
    [WGTDoc]                 VARCHAR (50)   NULL,
    [Fk_Enum_CRCategory]     INT            NULL,
    [Fk_Specification]       INT            NULL,
    [Fk_Release]             INT            NULL,
    [Fk_CurrentVersion]      INT            NULL,
    [Fk_NewVersion]          INT            NULL,
    [Fk_Impact]              INT            NULL,
    CONSTRAINT [PK_ChangeRequest] PRIMARY KEY CLUSTERED ([Pk_ChangeRequest] ASC),
    CONSTRAINT [FK_ChangeRequest_TsgStatus] FOREIGN KEY ([Fk_TSGStatus]) REFERENCES [dbo].[Enum_ChangeRequestStatus] ([Pk_EnumChangeRequestStatus]),
    CONSTRAINT [FK_ChangeRequest_WgStatus] FOREIGN KEY ([Fk_WGStatus]) REFERENCES [dbo].[Enum_ChangeRequestStatus] ([Pk_EnumChangeRequestStatus]),
    CONSTRAINT [FK_CRCurrentVersion] FOREIGN KEY ([Fk_CurrentVersion]) REFERENCES [dbo].[Version] ([Pk_VersionId]),
    CONSTRAINT [FK_CRNewVersion] FOREIGN KEY ([Fk_NewVersion]) REFERENCES [dbo].[Version] ([Pk_VersionId]),
    CONSTRAINT [FK_CRRelease] FOREIGN KEY ([Fk_Release]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [FK_CRSpecification] FOREIGN KEY ([Fk_Specification]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId]),
    CONSTRAINT [FK_Enum_CRCategory] FOREIGN KEY ([Fk_Enum_CRCategory]) REFERENCES [dbo].[Enum_CRCategory] ([Pk_EnumCRCategory]),
    CONSTRAINT [FK_Enum_CRImpact] FOREIGN KEY ([Fk_Impact]) REFERENCES [dbo].[Enum_CRImpact] ([Pk_EnumCRImpact])
);
































GO
CREATE NONCLUSTERED INDEX [IX_ChangeRequest]
    ON [dbo].[ChangeRequest]([Pk_ChangeRequest] ASC);

