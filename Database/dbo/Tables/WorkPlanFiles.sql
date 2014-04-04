CREATE TABLE [dbo].[WorkPlanFiles] (
    [Pk_WorkPlanFileId] INT            IDENTITY (1, 1) NOT NULL,
    [CreationDate]      DATETIME       NOT NULL,
    [WorkPlanFilePath]  NVARCHAR (255) NOT NULL
);

