USE [U3GPPDB]
GO

/****** Object:  Table [dbo].[ShortUrl]    Script Date: 03/14/2014 08:57:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ShortUrl](
	[Pk_Id] [int] NOT NULL,
	[Token] [varchar](50) NOT NULL,
	[Url] [varchar](200) NOT NULL,
 CONSTRAINT [PK_ShortUrl] PRIMARY KEY CLUSTERED 
(
	[Pk_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


