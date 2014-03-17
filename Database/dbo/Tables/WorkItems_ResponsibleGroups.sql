CREATE TABLE [dbo].[WorkItems_ResponsibleGroups](
	[Pk_WorkItemResponsibleGroups] [int] IDENTITY(1,1) NOT NULL,
	[Fk_WorkItemId] [int] NULL,
	[Fk_TbId] [int] NULL,
	[ResponsibleGroup] [varchar](5) NULL,
	[IsPrimeResponsible] [bit] NULL,
    CONSTRAINT [PK_WorkItems_ResponsibleGroups] PRIMARY KEY CLUSTERED ([Pk_WorkItemResponsibleGroups] ASC),
    CONSTRAINT [FK_WorkItems_ResponsibleGroups_WorkItems] FOREIGN KEY ([Fk_WorkItemId]) REFERENCES [dbo].[WorkItems] ([Pk_WorkItemUid])
);

