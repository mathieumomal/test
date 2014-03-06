USE [U3GPPDB]
GO
/****** Object:  Table [dbo].[WorkItems]    Script Date: 03/05/2014 14:36:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkItems](
	[Pk_WorkItemUid] [int] NOT NULL,
	[WorkplanId] [int] NULL,
	[Name] [varchar](255) NULL,
	[Acronym] [varchar](50) NULL,
	[WiLevel] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Completion] [int] NULL,
	[LastModification] [datetime] NULL,
	[CreationDate] [datetime] NULL,
	[Fk_ParentWiId] [int] NULL,
	[Fk_ReleaseId] [int] NULL,
	[Wid] [varchar](20) NULL,
	[RapporteurId] [int] NULL,
	[RapporteurStr] [varchar](100) NULL,
	[TsgApprovalMtgId] [int] NULL,
	[TsgApprovalMtgRef] [varchar](20) NULL,
	[PcgApprovalMtgId] [int] NULL,
	[PcgApprovalMtgRef] [varchar](20) NULL,
	[TsgStoppedMtgId] [int] NULL,
	[TsgStoppedMtgRef] [varchar](20) NULL,
	[PcgStoppedMtgId] [int] NULL,
	[PcgStoppedMtgRef] [varchar](20) NULL,
	[StatusReport] [varchar](50) NULL,
	[RapporteurCompany] [varchar](100) NULL,
	[TssAndTrs] [varchar](50) NULL,
 CONSTRAINT [PK_WorkItems] PRIMARY KEY CLUSTERED 
(
	[Pk_WorkItemUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_WorkItems] UNIQUE NONCLUSTERED 
(
	[Fk_ParentWiId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[WorkItems]  WITH CHECK ADD  CONSTRAINT [FK_WorkItems_ParentWi] FOREIGN KEY([Fk_ParentWiId])
REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
GO
ALTER TABLE [dbo].[WorkItems] CHECK CONSTRAINT [FK_WorkItems_ParentWi]
GO
ALTER TABLE [dbo].[WorkItems]  WITH CHECK ADD  CONSTRAINT [FK_WorkItems_Releases] FOREIGN KEY([Fk_ReleaseId])
REFERENCES [dbo].[Releases] ([Pk_ReleaseId])
GO
ALTER TABLE [dbo].[WorkItems] CHECK CONSTRAINT [FK_WorkItems_Releases]
GO
