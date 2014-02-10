USE [3GPPDB]
GO

/****** Object:  Table [dbo].[Releases]    Script Date: 02/10/2014 11:17:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Releases](
	[ReleaseId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nchar](50) NULL,
	[ShortName] [nchar](20) NULL,
	[Description] [nchar](200) NULL,
	[Fk_ReleaseStatus] [int] NULL,
	[StartDate] [datetime] NULL,
	[Stage1FreezeDate] [datetime] NULL,
	[Stage1FreezeMtgId] [int] NULL,
	[Stage2FreezeDate] [datetime] NULL,
	[Stage2FreezeMtgId] [int] NULL,
	[Stage3FreezeDate] [datetime] NULL,
	[Stage3FreezeMtgId] [int] NULL,
	[EndDate] [datetime] NULL,
	[EndMtgId] [int] NULL,
	[ClosureDate] [datetime] NULL,
	[ClosureMtgId] [int] NULL,
	[SortOrder] [int] NULL,
	[Version2g] [int] NULL,
	[Version3g] [int] NULL,
	[WpmCode2g] [nchar](50) NULL,
	[WpmCode3g] [nchar](50) NULL,
	[WpmProjectId] [int] NULL,
	[IturCode] [nchar](20) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Releases]  WITH CHECK ADD  CONSTRAINT [FK_Releases_Enum_ReleaseStatus] FOREIGN KEY([Fk_ReleaseStatus])
REFERENCES [dbo].[Enum_ReleaseStatus] ([Enum_ReleaseStatusId])
GO

ALTER TABLE [dbo].[Releases] CHECK CONSTRAINT [FK_Releases_Enum_ReleaseStatus]
GO


