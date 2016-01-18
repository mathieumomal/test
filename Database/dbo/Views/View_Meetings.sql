



CREATE VIEW [dbo].[View_Meetings]
AS
SELECT     MTG_ID, MTG_CATEG_CODE, MTG_TYPE_CODE, MTGS_CODE, MTG_REF, START_DATE, END_DATE, MTG_YEAR, MTG_SEQNO, MTG_TITLE, OWNERSHIP_LEVEL, INVITATION_DATE, DEADLINE_DATE, DURATION, INVITED_NUMB, ATTENDANT_NUMB, LOCAL_FLG, ORGA_ID, 
                      LOC_ADDRESS, LOC_ZIP, LOC_CITY, LOC_CTY_CODE, MTGREF_MASK, START_REGISTRATION_DATE, LOCATION_DETAILS_URL, TB_ID, 
                      Creation_Date, csn.ShortName, [dbo].[getMeetingShortRef](MEETINGS_1.MTG_REF) AS MtgShortRef
FROM         [$(DSDB)].dbo.MEETINGS AS MEETINGS_1
INNER JOIN dbo.Enum_CommunitiesShortName csn ON csn.Fk_TbId = TB_ID
WHERE     (TB_ID IN
                          (SELECT     TB_ID
                            FROM          [$(DSDB)].dbo.TAB_TB
                            WHERE      (TB_KEY1 = '3GPP') OR (TB_KEY1 ='SMG') OR (TB_KEY1 = 'GSM')))
GO
