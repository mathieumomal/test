CREATE TABLE [dbo].[Enum_CRCategory] (
    [Pk_EnumCRCategory] INT           IDENTITY (1, 1) NOT NULL,
    [Code]              VARCHAR (5)   NOT NULL,
    [Description]       VARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Enum_CRCategory] PRIMARY KEY CLUSTERED ([Pk_EnumCRCategory] ASC)
);



