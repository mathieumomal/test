﻿CREATE TABLE [dbo].[CR_Version] (
    [Pk_CRVersion] INT IDENTITY (1, 1) NOT NULL,
    [Fk_CR]        INT NOT NULL,
    [Fk_Version]   INT NOT NULL,
    [IsNew]        BIT NOT NULL,
    CONSTRAINT [PK_CR_Versions] PRIMARY KEY CLUSTERED ([Pk_CRVersion] ASC),
    CONSTRAINT [FK_CR] FOREIGN KEY ([Fk_CR]) REFERENCES [dbo].[ChangeRequest] ([Pk_ChangeRequest]),
    CONSTRAINT [FK_Version] FOREIGN KEY ([Fk_Version]) REFERENCES [dbo].[Version] ([Pk_VersionId])
);









