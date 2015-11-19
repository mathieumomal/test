CREATE TABLE [dbo].[SpecificationsGenealogy] (
    [Fk_SpecificationId]       INT NOT NULL,
    [Fk_SpecificationParentId] INT NOT NULL,
    CONSTRAINT [PK_SpecificationGenealogy] PRIMARY KEY CLUSTERED ([Fk_SpecificationId] ASC, [Fk_SpecificationParentId] ASC),
    CONSTRAINT [FK_SpecifcationChild] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId]),
    CONSTRAINT [FK_SpecifcationParent] FOREIGN KEY ([Fk_SpecificationParentId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);

