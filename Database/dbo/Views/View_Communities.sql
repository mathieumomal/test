﻿CREATE VIEW [dbo].[View_Communities]
AS
SELECT tb.TB_ID as TbId, tb.TB_NAME as TbName, tb.PARENT_TB_ID as ParentTbId, tb.TBS_CODE as ActiveCode, csn.ShortName as ShortName
FROM [$(DSDB)].dbo.TAB_TB tb
INNER JOIN Enum_CommunitiesShortName csn on csn.Fk_TbId = tb.TB_ID

WHERE tb.TB_KEY1='3GPP' or tb.TB_KEY1='SMG'