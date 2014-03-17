CREATE TABLE [dbo].[ShortUrl](
	[Pk_Id] [int] NOT NULL,
	[Token] [varchar](50) NOT NULL,
	[Url] [varchar](200) NOT NULL,
    CONSTRAINT [PK_ShortUrl] PRIMARY KEY CLUSTERED ([Pk_Id] ASC)
 );