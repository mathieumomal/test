USE [U3GPPDB]
GO
/****** Object:  Table [dbo].[Releases]    Script Date: 02/17/2014 16:10:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Releases](
	[Pk_ReleaseId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](10) NULL,
	[Name] [nvarchar](50) NULL,
	[ShortName] [nvarchar](50) NULL,
	[Description] [nvarchar](200) NULL,
	[Fk_ReleaseStatus] [int] NULL,
	[StartDate] [datetime] NULL,
	[Stage1FreezeDate] [datetime] NULL,
	[Stage1FreezeMtgRef] [nvarchar](20) NULL,
	[Stage1FreezeMtgId] [int] NULL,
	[Stage2FreezeDate] [datetime] NULL,
	[Stage2FreezeMtgRef] [nvarchar](20) NULL,
	[Stage2FreezeMtgId] [int] NULL,
	[Stage3FreezeMtgRef] [nvarchar](20) NULL,
	[Stage3FreezeDate] [datetime] NULL,
	[Stage3FreezeMtgId] [int] NULL,
	[EndDate] [datetime] NULL,
	[EndMtgRef] [nvarchar](20) NULL,
	[EndMtgId] [int] NULL,
	[ClosureDate] [datetime] NULL,
	[ClosureMtgRef] [nvarchar](20) NULL,
	[ClosureMtgId] [int] NULL,
	[SortOrder] [int] NULL,
	[Version2g] [int] NULL,
	[Version3g] [int] NULL,
	[WpmCode2g] [nvarchar](50) NULL,
	[WpmCode3g] [nvarchar](50) NULL,
	[WpmProjectId] [int] NULL,
	[IturCode] [nvarchar](20) NULL,
	[LAST_MOD_BY] [nvarchar](20) NULL,
	[LAST_MOD_TS] [datetime] NULL,
 CONSTRAINT [PK_Releases] PRIMARY KEY CLUSTERED 
(
	[Pk_ReleaseId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Releases]  WITH CHECK ADD  CONSTRAINT [FK_Releases_Enum_ReleaseStatus] FOREIGN KEY([Fk_ReleaseStatus])
REFERENCES [dbo].[Enum_ReleaseStatus] ([Enum_ReleaseStatusId])
GO
ALTER TABLE [dbo].[Releases] CHECK CONSTRAINT [FK_Releases_Enum_ReleaseStatus]
GO
