CREATE TABLE [dbo].[Version] (
    [Pk_VersionId]            INT           IDENTITY (1, 1) NOT NULL,
    [MajorVersion]            INT           NULL,
    [TechnicalVersion]        INT           NULL,
    [EditorialVersion]        INT           NULL,
    [AchievedDate]            DATETIME      NULL,
    [ExpertProvided]          DATETIME      NULL,
    [Location]                VARCHAR (150) NULL,
    [SupressFromSDO_Pub]      BIT           NOT NULL,
    [ForcePublication]        BIT           NOT NULL,
    [DocumentUploaded]        DATETIME      NULL,
    [DocumentPassedToPub]     DATETIME      NULL,
    [Multifile]               BIT           NOT NULL,
    [Source]                  INT           NULL,
    [ETSI_WKI_ID]             INT           NULL,
    [ProvidedBy]              INT           NULL,
    [Fk_SpecificationId]      INT           NULL,
    [Fk_ReleaseId]            INT           NULL,
    [ETSI_WKI_Ref]            AS            ([dbo].[getETSI_WKI_Ref]([ETSI_WKI_ID])),
    [RelatedTDoc]             VARCHAR (200) NULL,
    [SupressFromMissing_List] BIT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [Pk_VersionId] PRIMARY KEY CLUSTERED ([Pk_VersionId] ASC),
    CONSTRAINT [Version_FK_ReleaseID] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [Version_FK_SpecificationID] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);







