CREATE TABLE [dbo].[Enum_CommunitiesShortName](
	[Pk_EnumCommunitiesShortNames] [int] IDENTITY(1,1) NOT NULL,
	[Fk_TbId] [int] NULL,
	[ShortName] [varchar](10) NULL,
 CONSTRAINT [PK_Enum_CommunitiesShortName] PRIMARY KEY CLUSTERED ([Pk_EnumCommunitiesShortNames] ASC)
);

