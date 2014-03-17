CREATE FUNCTION [dbo].[getPersonName](@personId int)
RETURNS varchar(50)
AS
BEGIN
    DECLARE @result varchar(50)
    select @result = (FIRSTNAME + ' '+ LASTNAME) from [$(DSDB)].dbo.PERSON where PERSON_ID = @personId
    RETURN @result
END
GO
