-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Database_Reset] 
AS
BEGIN
	exec CR_CleanAll;
	exec Versions_CleanAll;
	exec Specifications_CleanAll;
	exec WorkItems_CleanAll;
	DELETE FROM Remarks
	DBCC CHECKIDENT('Remarks', RESEED, 0)
	DELETE FROM History
	DBCC CHECKIDENT('History', RESEED, 0)
	DELETE FROM Releases
	DBCC CHECKIDENT('Releases', RESEED, 0)
	DELETE FROM ShortUrl
	DBCC CHECKIDENT('ShortUrl', RESEED, 0)
	DELETE FROM SyncInfo
	DBCC CHECKIDENT('SyncInfo', RESEED, 0)
END