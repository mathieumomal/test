CREATE TABLE [dbo].[SpecifcationsGenealogy] (
    [Fk_SpecificationId]       INT NOT NULL,
    [Fk_SpecificationParentId] INT NOT NULL,
    CONSTRAINT [PK_SpecifcationsGenealogy] PRIMARY KEY CLUSTERED ([Fk_SpecificationId] ASC, [Fk_SpecificationParentId] ASC)
);

