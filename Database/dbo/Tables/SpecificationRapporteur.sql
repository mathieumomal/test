CREATE TABLE [dbo].[SpecificationRapporteur] (
    [Pk_SpecificationRapporteurId] INT IDENTITY (1, 1) NOT NULL,
    [Fk_SpecificationId]           INT NOT NULL,
    [Fk_RapporteurId]              INT NOT NULL,
    [IsPrime]                      BIT NOT NULL,
    CONSTRAINT [PK_SpecificationRapporteur] PRIMARY KEY CLUSTERED ([Pk_SpecificationRapporteurId] ASC),
    CONSTRAINT [Raporteur_FK_SpecificationID] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);



