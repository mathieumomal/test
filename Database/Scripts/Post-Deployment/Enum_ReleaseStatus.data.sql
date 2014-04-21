DECLARE @Enum_ReleaseStatusId INT

IF NOT EXISTS (SELECT 1 FROM dbo.Enum_ReleaseStatus WHERE code = 'Open')
BEGIN
    SELECT @Enum_ReleaseStatusId = MAX(ISNULL(Enum_ReleaseStatusId, 0)) + 1 from dbo.Enum_ReleaseStatus
    INSERT INTO dbo.Enum_ReleaseStatus (Enum_ReleaseStatusId, code, [Description]) VALUES (@Enum_ReleaseStatusId, 'Open', 'Open')
END

IF NOT EXISTS (SELECT 1 FROM dbo.Enum_ReleaseStatus WHERE code = 'Frozen')
BEGIN
    SELECT @Enum_ReleaseStatusId = MAX(ISNULL(Enum_ReleaseStatusId, 0)) + 1 from dbo.Enum_ReleaseStatus
    INSERT INTO dbo.Enum_ReleaseStatus (Enum_ReleaseStatusId, code, [Description]) VALUES (@Enum_ReleaseStatusId,'Frozen', 'Frozen')
END

IF NOT EXISTS (SELECT 1 FROM dbo.Enum_ReleaseStatus WHERE code = 'Closed')
BEGIN
    SELECT @Enum_ReleaseStatusId = MAX(ISNULL(Enum_ReleaseStatusId, 0)) + 1 from dbo.Enum_ReleaseStatus
    INSERT INTO dbo.Enum_ReleaseStatus (Enum_ReleaseStatusId, code, [Description]) VALUES (@Enum_ReleaseStatusId,'Closed', 'Closed')
END