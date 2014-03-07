USE [U3GPPDB]
GO

/****** Object:  View [dbo].[View_Communities]    Script Date: 03/07/2014 15:15:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[View_Communities]
AS
SELECT tb.TB_ID as TbId, tb.TB_NAME as TbName, tb.PARENT_TB_ID as ParentTbId, tb.TBS_CODE as ActiveCode, csn.ShortName as ShortName
FROM DSDB..TAB_TB tb
INNER JOIN Enum_CommunitiesShortName csn on csn.Fk_TbId = tb.TB_ID

WHERE tb.TB_KEY1='3GPP' or tb.TB_KEY1='SMG'



GO


