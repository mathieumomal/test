CREATE TABLE [dbo].[SpecificationResponsibleGroup] (
    [Pk_SpecificationResponsibleGroupId] INT NOT NULL,
    [Fk_SpecificationId]                 INT NOT NULL,
    [Fk_commityId]                       INT NOT NULL,
    [IsPrime]                            BIT NOT NULL,
    CONSTRAINT [PK_SpecificationResponsibleGroup] PRIMARY KEY CLUSTERED ([Pk_SpecificationResponsibleGroupId] ASC),
    CONSTRAINT [RG_FK_SpecificationID] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);

