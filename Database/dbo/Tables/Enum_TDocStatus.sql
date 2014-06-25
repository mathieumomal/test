CREATE TABLE [dbo].[Enum_TDocStatus] (
    [Pk_EnumTDocStatus] INT           IDENTITY (1, 1) NOT NULL,
    [Code]              VARCHAR (50)  NOT NULL,
    [SortOrder]         INT           NOT NULL,
    [Description]       VARCHAR (200) NOT NULL,
    [WGUsable]          BIT           NOT NULL,
    [TSGUsable]         BIT           NOT NULL,
    CONSTRAINT [PK_Enum_TDocStatus] PRIMARY KEY CLUSTERED ([Pk_EnumTDocStatus] ASC)
);



