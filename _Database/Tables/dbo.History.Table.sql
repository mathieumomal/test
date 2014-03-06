USE [U3GPPDB]
GO
/****** Object:  Table [dbo].[History]    Script Date: 03/05/2014 14:36:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[History](
	[Pk_HistoryId] [int] IDENTITY(1,1) NOT NULL,
	[Fk_ReleaseId] [int] NULL,
	[Fk_PersonId] [int] NULL,
	[CreationDate] [datetime] NULL,
	[HistoryText] [nvarchar](255) NULL,
	[PersonName]  AS ([dbo].[getPersonName]([Fk_PersonId])),
 CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED 
(
	[Pk_HistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_Releases] FOREIGN KEY([Fk_ReleaseId])
REFERENCES [dbo].[Releases] ([Pk_ReleaseId])
GO
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_Releases]
GO
