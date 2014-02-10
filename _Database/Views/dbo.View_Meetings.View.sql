USE [3GPPDB]
GO
/****** Object:  View [dbo].[View_Meetings]    Script Date: 02/10/2014 11:10:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_Meetings]
AS
SELECT     MTG_ID, MTG_CATEG_CODE, MTG_TYPE_CODE, MTGS_CODE, MTG_REF, START_DATE, END_DATE, MTG_YEAR, MTG_SEQNO, MTG_TITLE, RESP_DEPT, 
                      OWNER_ID, OWNER_DEPT, OWNERSHIP_LEVEL, INVITATION_DATE, DEADLINE_DATE, DURATION, INVITED_NUMB, ATTENDANT_NUMB, LOCAL_FLG, ORGA_ID, 
                      LOC_ADDRESS, LOC_ZIP, LOC_CITY, LOC_CTY_CODE, BUILDING, LOC_FREE_TEXT, SUM_REF, SUM_REC_FLG, SUM_REC_DATE, SUM_DESP_DATE, MAIN_REF, 
                      MAIN_REC_FLG, MAIN_REC_DATE, MAIN_DESP_DATE, SUB_GROUP_CODE, MTGREF_MASK, OWNER_LIST_ID, MOD_TS, MOD_BY, DOC_PERCENT_PAPER, 
                      DOC_PERCENT_ELECTRONIC, START_REGISTRATION_DATE, LOCATION_DETAILS_URL, WEB_MOD_BY, WEB_MOD_TS, REQUESTER_ID, REQUEST_DATE, TB_ID, 
                      Creation_Date
FROM         DSDB.dbo.MEETINGS AS MEETINGS_1
WHERE     (TB_ID IN
                          (SELECT     TB_ID
                            FROM          DSDB.dbo.TAB_TB
                            WHERE      (TB_KEY1 = '3GPP')))
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[28] 2[31] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "MEETINGS_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 319
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_Meetings'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_Meetings'
GO
