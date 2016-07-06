CREATE TABLE [dbo].[Enum_CommunitiesShortName] (
    [Pk_EnumCommunitiesShortNames] INT          IDENTITY (1, 1) NOT NULL,
    [Fk_TbId]                      INT          NULL,
    [ShortName]                    VARCHAR (10) NULL,
    [WpmProjectId]                 INT          NULL,
    [MapId_3SS]                    INT          NULL,
    CONSTRAINT [PK_Enum_CommunitiesShortName] PRIMARY KEY CLUSTERED ([Pk_EnumCommunitiesShortNames] ASC)
);





