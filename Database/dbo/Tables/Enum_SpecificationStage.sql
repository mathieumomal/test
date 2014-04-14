CREATE TABLE [dbo].[Enum_SpecificationStage] (
    [Pk_Enum_SpecificationStage] INT          IDENTITY (1, 1) NOT NULL,
    [Code]                       VARCHAR (20) NOT NULL,
    [Description]                VARCHAR (50) NOT NULL,
    CONSTRAINT [Pk_Enum_SpecificationStage] PRIMARY KEY CLUSTERED ([Pk_Enum_SpecificationStage] ASC)
);

