CREATE TABLE [dbo].[CR_WorkItems] (
    [Pk_CRWorkItems] INT IDENTITY (1, 1) NOT NULL,
    [Fk_CRId]        INT NULL,
    [Fk_WIId]        INT NULL,
    CONSTRAINT [PK_CR_WorkItems] PRIMARY KEY CLUSTERED ([Pk_CRWorkItems] ASC),
    CONSTRAINT [FK_CRId] FOREIGN KEY ([Fk_CRId]) REFERENCES [dbo].[ChangeRequest] ([Pk_ChangeRequest]),
    CONSTRAINT [FK_WIId] FOREIGN KEY ([Fk_WIId]) REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
);



