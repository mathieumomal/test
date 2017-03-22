


CREATE FUNCTION [dbo].[getETSI_WKI_Ref](@ETSI_WKI_ID int)
RETURNS varchar(50)
AS
BEGIN
    DECLARE @result varchar(50)
    select @result = WKI_ETSI_NUMBER from U3GPPDB.dbo.ETSI_WorkItem where WKI_ID = @ETSI_WKI_ID
    RETURN @result
END