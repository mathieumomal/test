USE [3GPPDB]
GO

/****** Object:  Table [dbo].[Enum_ReleaseStatus]    Script Date: 02/10/2014 11:17:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Enum_ReleaseStatus](
	[Enum_ReleaseStatusId] [int] IDENTITY(1,1) NOT NULL,
	[ReleaseStatus] [nchar](10) NULL,
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


