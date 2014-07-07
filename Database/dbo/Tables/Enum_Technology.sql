CREATE TABLE [dbo].[Enum_Technology] (
    [Pk_Enum_TechnologyId] INT           IDENTITY (1, 1) NOT NULL,
    [Code]                 VARCHAR (50)  NOT NULL,
    [Description]          VARCHAR (150) NULL,
    [WpmProjectId]         INT           NULL,
    CONSTRAINT [PK_Enum_Technology] PRIMARY KEY CLUSTERED ([Pk_Enum_TechnologyId] ASC)
);



