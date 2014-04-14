CREATE TABLE [dbo].[Specification_WorkItem] (
    [Pk_Specification_WorkItemId] INT IDENTITY (1, 1) NOT NULL,
    [Fk_SpecificationId]          INT NOT NULL,
    [Fk_WorkItemId]               INT NOT NULL,
    [isPrime]                     BIT NULL,
    [IsSetByUser]                 BIT NULL,
    CONSTRAINT [Pk_Specification_WorkItemId] PRIMARY KEY CLUSTERED ([Pk_Specification_WorkItemId] ASC),
    CONSTRAINT [FK_SpecificationID] FOREIGN KEY ([Fk_SpecificationId]) REFERENCES [dbo].[Specification] ([Pk_SpecificationId]),
    CONSTRAINT [FK_WorkItemID] FOREIGN KEY ([Fk_WorkItemId]) REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
);

