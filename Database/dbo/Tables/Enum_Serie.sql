CREATE TABLE [dbo].[Enum_Serie] (
    [Pk_Enum_SerieId] INT          IDENTITY (1, 1) NOT NULL,
    [Code]            VARCHAR (20) NOT NULL,
    [Description]     VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Enum_Serie] PRIMARY KEY CLUSTERED ([Pk_Enum_SerieId] ASC)
);

