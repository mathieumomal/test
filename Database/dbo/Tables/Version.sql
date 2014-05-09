﻿CREATE TABLE [dbo].[Version] (
    [Pk_VersionId]         INT           IDENTITY (1, 1) NOT NULL,
    [MajorVersion]         VARCHAR (3)   NULL,
    [TechnicalVersion]     VARCHAR (3)   NULL,
    [EditorialVersion]     VARCHAR (3)   NULL,
    [Milestone]            VARCHAR (45)  NULL,
    [AchievedDate]         DATETIME      NULL,
    [ExpertProvided]       DATETIME      NULL,
    [Location]             VARCHAR (150) NULL,
    [SupressFromSDO_Pub]   BIT           NOT NULL,
    [ForcePublication]     BIT           NOT NULL,
    [DocumentChecked]      DATETIME      NULL,
    [DocumentUploaded]     DATETIME      NULL,
    [DocumentPassedToPub]  DATETIME      NULL,
    [SupressTransposition] BIT           NOT NULL,
    [Multifile]            BIT           NOT NULL,
    [Source]               INT           NULL,
    [ETSI_WKI_ID]          INT           NULL,
    [ProvidedBy]           INT           NULL,
    [Fk_SpecificationId]   INT           NOT NULL,
    [Fk_ReleaseId]         INT           NOT NULL,
    CONSTRAINT [Pk_VersionId] PRIMARY KEY CLUSTERED ([Pk_VersionId] ASC),
    CONSTRAINT [Version_FK_ReleaseID] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [Version_FK_SpecificationID] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);





