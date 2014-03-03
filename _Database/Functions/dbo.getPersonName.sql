/****** Object:  UserDefinedFunction [dbo].[getPersonName]    Script Date: 03/03/2014 16:05:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[getPersonName](@personId int)
RETURNS varchar(50)
AS
BEGIN
    DECLARE @result varchar(50)
    select @result = (FIRSTNAME + ' '+ LASTNAME) from DSDB.dbo.PERSON where PERSON_ID = @personId
    RETURN @result
END
GO