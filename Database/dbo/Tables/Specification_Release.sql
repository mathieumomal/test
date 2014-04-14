CREATE TABLE [dbo].[Specification_Release] (
    [Pk_Specification_ReleaseId] INT      IDENTITY (1, 1) NOT NULL,
    [isWithdrawn]                BIT      NULL,
    [CreationDate]               DATETIME NULL,
    [UpdateDate]                 DATETIME NULL,
    [Fk_SpecificationId]         INT      NOT NULL,
    [Fk_ReleaseId]               INT      NOT NULL,
    [FreezeMeetingId]            INT      NULL,
    [WithdrawMeetingId]          INT      NULL,
    CONSTRAINT [PK_Specification_Release] PRIMARY KEY CLUSTERED ([Pk_Specification_ReleaseId] ASC),
    CONSTRAINT [FK_ReleaseID] FOREIGN KEY ([Fk_ReleaseId]) REFERENCES [dbo].[Releases] ([Pk_ReleaseId]),
    CONSTRAINT [Release_FK_SpecificationID] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId])
);

