﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WorkItems_CleanAll] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	Declare @Error int


    -- Insert statements for procedure here
	DELETE FROM WorkItems_ResponsibleGroups;
	DBCC CHECKIDENT('WorkItems_ResponsibleGroups', RESEED, 0)
	DELETE FROM Remarks WHERE Fk_WorkItemId IS NOT NULL;
	DELETE FROM WorkItems;
	DELETE FROM WorkPlanFiles;
	DBCC CHECKIDENT('WorkPlanFiles', RESEED, 0)

	Select @Error = @@error
	Return @Error
END