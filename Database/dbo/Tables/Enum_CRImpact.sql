CREATE TABLE [dbo].[Enum_CRImpact] (
    [Pk_EnumCRImpact] INT          IDENTITY (1, 1) NOT NULL,
    [Code]            VARCHAR (50) NOT NULL,
    [Description]     NCHAR (10)   NULL,
    CONSTRAINT [PK_Enum_CRImpact] PRIMARY KEY CLUSTERED ([Pk_EnumCRImpact] ASC)
);





