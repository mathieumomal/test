USE [U3GPPDB]
GO
/****** Object:  Table [dbo].[Remarks]    Script Date: 03/05/2014 14:36:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Remarks](
	[Pk_RemarkId] [int] IDENTITY(1,1) NOT NULL,
	[Fk_ReleaseId] [int] NULL,
	[Fk_WorkItemId] [int] NULL,
	[Fk_PersonId] [int] NULL,
	[IsPublic] [bit] NULL,
	[CreationDate] [datetime] NULL,
	[RemarkText] [nvarchar](255) NULL,
	[PersonName]  AS ([dbo].[getPersonName]([Fk_PersonId])),
 CONSTRAINT [PK_Remarks] PRIMARY KEY CLUSTERED 
(
	[Pk_RemarkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Remarks]  WITH CHECK ADD  CONSTRAINT [FK_Remarks_Releases] FOREIGN KEY([Fk_ReleaseId])
REFERENCES [dbo].[Releases] ([Pk_ReleaseId])
GO
ALTER TABLE [dbo].[Remarks] CHECK CONSTRAINT [FK_Remarks_Releases]
GO
ALTER TABLE [dbo].[Remarks]  WITH CHECK ADD  CONSTRAINT [FK_Remarks_WorkItems] FOREIGN KEY([Fk_WorkItemId])
REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
GO
ALTER TABLE [dbo].[Remarks] CHECK CONSTRAINT [FK_Remarks_WorkItems]
GO
ALTER TABLE [dbo].[Remarks] ADD  CONSTRAINT [DF_Remarks_IsPublic]  DEFAULT ((1)) FOR [IsPublic]
GO
