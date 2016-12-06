CREATE TABLE [dbo].[WorkItems] (
    [Pk_WorkItemUid]             INT           NOT NULL,
    [WorkplanId]                 INT           NULL,
    [Name]                       VARCHAR (255) NULL,
    [Acronym]                    VARCHAR (50)  NULL,
    [WiLevel]                    INT           NULL,
    [StartDate]                  DATETIME      NULL,
    [EndDate]                    DATETIME      NULL,
    [Completion]                 INT           NULL,
    [LastModification]           DATETIME      NULL,
    [CreationDate]               DATETIME      NULL,
    [Fk_ParentWiId]              INT           NULL,
    [Fk_ReleaseId]               INT           NULL,
    [Wid]                        VARCHAR (20)  NULL,
    [RapporteurId]               INT           NULL,
    [RapporteurStr]              VARCHAR (100) NULL,
    [TsgApprovalMtgId]           INT           NULL,
    [TsgApprovalMtgRef]          VARCHAR (20)  NULL,
    [PcgApprovalMtgId]           INT           NULL,
    [PcgApprovalMtgRef]          VARCHAR (20)  NULL,
    [TsgStoppedMtgId]            INT           NULL,
    [TsgStoppedMtgRef]           VARCHAR (20)  NULL,
    [PcgStoppedMtgId]            INT           NULL,
    [PcgStoppedMtgRef]           VARCHAR (20)  NULL,
    [StatusReport]               VARCHAR (50)  NULL,
    [RapporteurCompany]          VARCHAR (100) NULL,
    [TssAndTrs]                  VARCHAR (50)  NULL,
    [Effective_Acronym]          VARCHAR (50)  NULL,
    [ImportCreationDate]         DATETIME      CONSTRAINT [DF_WorkItems_ImportCreationDate] DEFAULT ('2016-11-03 00:00:00.000') NOT NULL,
    [ImportLastModificationDate] DATETIME      CONSTRAINT [DF_WorkItems_ImportLastModificationDate] DEFAULT ('2016-11-03 00:00:00.000') NOT NULL,
    [DeletedFlag]                BIT           CONSTRAINT [DF_WorkItems_DeletedFlag] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_WorkItems] PRIMARY KEY CLUSTERED ([Pk_WorkItemUid] ASC),
    CONSTRAINT [FK_WorkItems_ParentWi] FOREIGN KEY ([Fk_ParentWiId]) REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid]),
    CONSTRAINT [FK_WorkItems_Releases] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId])
);







