USE [U3GPPDB]
GO

/****** Object:  View [dbo].[View_Persons]    Script Date: 03/04/2014 12:59:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[View_Persons]
AS
SELECT p.PERSON_ID, p.VMS_USER as Username, p.INTERNET_ADD as Email, p.LASTNAME, p.FIRSTNAME, o.ORGA_ID, o.ORGA_NAME
FROM DSDB..PERSON p
INNER JOIN DSDB..ORGANISATION o on o.ORGA_ID = p.ORGA_ID

WHERE p.DELETED_FLG='N' 


GO


