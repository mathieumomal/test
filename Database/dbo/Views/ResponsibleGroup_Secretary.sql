

CREATE VIEW [dbo].[ResponsibleGroup_Secretary]
AS
SELECT DISTINCT o.TB_ID AS TbId, p.INTERNET_ADD AS Email, i.PERSON_ID AS PersonId, i.END_DATE AS roleExpirationDate
FROM         [$(DSDB)].dbo.PERSON_IN_LIST AS i INNER JOIN
                      [$(DSDB)].dbo.PERSON_LIST AS o ON o.PLIST_ID = i.PLIST_ID AND i.PERS_ROLE_CODE = 'SECRETARY' AND o.PLIST_TYPE = 'MASTER' INNER JOIN
                      [$(DSDB)].dbo.PERSON AS p ON p.PERSON_ID = i.PERSON_ID
                      WHERE o.TB_ID IN (SELECT c.TbId from dbo.View_Communities as c);
GO