CREATE TABLE [dbo].[Specification] (
    [Pk_SpecificationId]    INT            IDENTITY (1, 1) NOT NULL,
    [Type]                  BIT            NULL,
    [Number]                VARCHAR (20)   NULL,
    [IsActive]              BIT            NOT NULL,
    [IsUnderChangeControl]  BIT            NULL,
    [promoteInhibited]      BIT            NULL,
    [IsForPublication]      BIT            NULL,
    [Title]                 VARCHAR (2000) NULL,
    [ComIMS]                BIT            NULL,
    [EPS]                   BIT            NULL,
    [_2gCommon]             BIT            NULL,
    [DefinitivelyWithdrawn] BIT            NULL,
    [CreationDate]          DATETIME       NULL,
    [UpdateDate]            DATETIME       NULL,
    [UpdateBy]              VARBINARY (50) NULL,
    [TitleVerified]         DATETIME       NULL,
    [URL]                   VARCHAR (256)  NULL,
    [ITU_Description]       VARCHAR (200)  NULL,
    [Fk_SerieId]            INT            NULL,
    CONSTRAINT [PK_Specification] PRIMARY KEY CLUSTERED ([Pk_SpecificationId] ASC),
    CONSTRAINT [FK_SerieID] FOREIGN KEY ([Fk_SerieId]) REFERENCES [dbo].[Enum_Serie] ([Pk_Enum_SerieId])
);

