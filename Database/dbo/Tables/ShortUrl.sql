CREATE TABLE [dbo].[ShortUrl] (
    [Pk_Id] INT           IDENTITY (1, 1) NOT NULL,
    [Token] VARCHAR (50)  NOT NULL,
    [Url]   VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ShortUrl] PRIMARY KEY CLUSTERED ([Pk_Id] ASC)
);

