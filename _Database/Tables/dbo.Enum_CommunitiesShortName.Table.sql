USE [U3GPPDB]
GO
/****** Object:  Table [dbo].[Enum_CommunitiesShortName]    Script Date: 03/05/2014 14:36:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Enum_CommunitiesShortName](
	[Pk_EnumCommunitiesShortNames] [int] IDENTITY(1,1) NOT NULL,
	[Fk_TbId] [int] NULL,
	[ShortName] [varchar](10) NULL,
 CONSTRAINT [PK_Enum_CommunitiesShortName] PRIMARY KEY CLUSTERED 
(
	[Pk_EnumCommunitiesShortNames] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
