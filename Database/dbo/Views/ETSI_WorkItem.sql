

CREATE VIEW [dbo].[ETSI_WorkItem]
AS
SELECT w.WKI_ID, w.WKI_REFERENCE, case WHEN w.CURRENT_STATUS_CODE1 = 12 THEN 1 ELSE 0 END as published, w.CURRENT_STATE_DATE as PublicationDate,
 w.ETSI_STANDARD_TYPE as StandardType, w.ETSI_NUMBER as Number, EDSPDFfilename as [fileName], EDSpathname as filePath
 FROM [$(WPMDB)].[dbo].[WORK_ITEM] w
 LEFT OUTER JOIN [$(WPMDB)].[dbo].Catalog_Main c on w.WKI_ID = c.WKI_ID