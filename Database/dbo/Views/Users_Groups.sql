﻿
CREATE VIEW [dbo].[Users_Groups]
AS
SELECT     P.PERSON_ID, PL.PLIST_ID, PL.TB_ID, PIL.PERS_ROLE_CODE, P.INTERNET_ADD, PIL.END_DATE
FROM [$(DSDB)].dbo.PERSON_LIST PL 
INNER JOIN [$(DSDB)].dbo.PERSON_IN_LIST PIL ON PIL.PLIST_ID = PL.PLIST_ID 
INNER JOIN [$(DSDB)].dbo.PERSON P ON PIL.PERSON_ID = P.PERSON_ID

WHERE P.DELETED_FLG='N'
AND (PL.PLIST_TYPE='MASTER' OR PL.PLIST_TYPE='OTHER')