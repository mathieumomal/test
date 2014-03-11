USE [U3GPPDB]
GO

/****** Object:  Table [dbo].[Enum_ReleaseStatus]    Script Date: 03/11/2014 14:42:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Enum_ReleaseStatus](
	[Enum_ReleaseStatusId] [int] IDENTITY(1,1) NOT NULL,
	[code] [varchar](10) NULL,
	[description] [varchar](50) NULL,
 CONSTRAINT [PK_Enum_ReleaseStatus] PRIMARY KEY CLUSTERED 
(
	[Enum_ReleaseStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_Enum_ReleaseStatus] UNIQUE NONCLUSTERED 
(
	[Enum_ReleaseStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


