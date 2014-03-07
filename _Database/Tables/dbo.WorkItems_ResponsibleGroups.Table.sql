USE [U3GPPDB]
GO
/****** Object:  Table [dbo].[WorkItems_ResponsibleGroups]    Script Date: 03/07/2014 12:18:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkItems_ResponsibleGroups](
	[Pk_WorkItemResponsibleGroups] [int] IDENTITY(1,1) NOT NULL,
	[Fk_WorkItemId] [int] NULL,
	[Fk_TbId] [int] NULL,
	[ResponsibleGroup] [varchar](5) NULL,
	[IsPrimeResponsible] [bit] NULL,
 CONSTRAINT [PK_WorkItems_ResponsibleGroups] PRIMARY KEY CLUSTERED 
(
	[Pk_WorkItemResponsibleGroups] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[WorkItems_ResponsibleGroups]  WITH CHECK ADD  CONSTRAINT [FK_WorkItems_ResponsibleGroups_WorkItems] FOREIGN KEY([Fk_WorkItemId])
REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
GO
ALTER TABLE [dbo].[WorkItems_ResponsibleGroups] CHECK CONSTRAINT [FK_WorkItems_ResponsibleGroups_WorkItems]
GO
