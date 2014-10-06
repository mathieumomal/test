CREATE TABLE [dbo].[Enum_ChangeRequestStatus] (
    [Pk_EnumChangeRequestStatus] INT          NOT NULL,
    [Code]                       VARCHAR (15) NULL,
    [Description]                VARCHAR (50) NULL,
    CONSTRAINT [PK_Enum_ChangeRequestStatus] PRIMARY KEY CLUSTERED ([Pk_EnumChangeRequestStatus] ASC)
);



