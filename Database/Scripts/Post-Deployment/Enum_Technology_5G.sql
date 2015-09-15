IF NOT EXISTS (SELECT 1 FROM dbo.Enum_Technology WHERE code = '2G')
BEGIN
    INSERT INTO dbo.Enum_Technology (code, [Description], WpmProjectId, SortOrder) VALUES ('2G', '2G', 744, 1)
END

IF NOT EXISTS (SELECT 1 FROM dbo.Enum_Technology WHERE code = '3G')
BEGIN
    INSERT INTO dbo.Enum_Technology (code, [Description], WpmProjectId, SortOrder) VALUES ('3G', '3G', 704, 2)
END

IF NOT EXISTS (SELECT 1 FROM dbo.Enum_Technology WHERE code = 'LTE')
BEGIN
    INSERT INTO dbo.Enum_Technology (code, [Description], WpmProjectId, SortOrder) VALUES ('LTE', 'LTE', 576, 3)
END

IF NOT EXISTS (SELECT 1 FROM dbo.Enum_Technology WHERE code = '5G')
BEGIN
    INSERT INTO dbo.Enum_Technology (code, [Description], WpmProjectId, SortOrder) VALUES ('5G', '5G', 704, 4)
END

UPDATE [U3GPPDB].[dbo].Enum_Technology SET SortOrder = 1 WHERE code = '2G'
UPDATE [U3GPPDB].[dbo].Enum_Technology SET SortOrder = 2 WHERE code = '3G'
UPDATE [U3GPPDB].[dbo].Enum_Technology SET SortOrder = 3 WHERE code = 'LTE'
UPDATE [U3GPPDB].[dbo].Enum_Technology SET SortOrder = 4 WHERE code = '5G'