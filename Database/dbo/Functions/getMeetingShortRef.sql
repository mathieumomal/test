
CREATE FUNCTION [dbo].[getMeetingShortRef](@shortName varchar(50), @meetingTitle varchar(100), @SeqNumb varchar(50), @MtgFullRef varchar(50))
RETURNS varchar(50)
AS
BEGIN
    DECLARE @result varchar(50)
    SET @result = @shortName+'-';
    IF (LEN(@SeqNumb)= 1)
		SET @result = @result +'0';
	SET @result = @result + CONVERT(varchar(20),@SeqNumb);
    IF(UPPER(@meetingTitle) LIKE 'BIS%' or UPPER(@MtgFullRef) LIKE '%BIS%')
		SET @result = @result + 'b';
    RETURN @result
END