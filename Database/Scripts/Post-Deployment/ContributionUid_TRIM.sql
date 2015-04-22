UPDATE dbo.ChangeRequest SET WGTDoc = RTRIM(LTRIM(WGTDoc));
UPDATE dbo.ChangeRequestTsgData SET TSGTdoc = RTRIM(LTRIM(TSGTdoc));