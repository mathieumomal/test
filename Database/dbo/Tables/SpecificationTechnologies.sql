CREATE TABLE [dbo].[SpecificationTechnologies] (
    [Pk_SpecificationTechnologyId] INT IDENTITY (1, 1) NOT NULL,
    [Fk_Specification]             INT NOT NULL,
    [Fk_Enum_Technology]           INT NOT NULL,
    CONSTRAINT [PK_SpecificationTechnologies] PRIMARY KEY CLUSTERED ([Pk_SpecificationTechnologyId] ASC),
    CONSTRAINT [FK_Enum_Technology] FOREIGN KEY ([Fk_Enum_Technology]) REFERENCES [dbo].[Enum_Technology] ([Pk_Enum_TechnologyId]),
    CONSTRAINT [FK_Specification] FOREIGN KEY ([Fk_Specification]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);

