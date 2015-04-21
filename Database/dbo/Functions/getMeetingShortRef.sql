

CREATE FUNCTION [dbo].[getMeetingShortRef](@MtgFullRef varchar(50))
RETURNS varchar(50)
AS
BEGIN
    DECLARE @result varchar(50)
    IF(UPPER(@MtgFullRef) LIKE '3GPP%')
		SET @result = RIGHT(@MtgFullRef, LEN(@MtgFullRef) - 4);
	ELSE
		SET @result = @MtgFullRef;
    RETURN @result
END