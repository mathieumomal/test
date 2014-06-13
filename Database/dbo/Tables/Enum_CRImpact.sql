CREATE TABLE [dbo].[Enum_CRImpact] (
    [Pk_EnumCRImpact] INT           IDENTITY (1, 1) NOT NULL,
    [Type]            VARCHAR (50)  NOT NULL,
    [Description]     VARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Enum_CRImpact] PRIMARY KEY CLUSTERED ([Pk_EnumCRImpact] ASC)
);

