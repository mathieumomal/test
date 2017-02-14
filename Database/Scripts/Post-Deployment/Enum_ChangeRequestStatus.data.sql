TRUNCATE TABLE dbo.Enum_ChangeRequestStatus

INSERT INTO dbo.Enum_ChangeRequestStatus(Pk_EnumChangeRequestStatus, Code, [Description]) VALUES
(1, 'Agreed', 'Agreed'),
(2, 'Approved','Approved'),
(3, 'Noted','Noted'),
(4, 'Postponed','Postponed'),
(5, 'Rejected','Rejected'),
(6, 'Revised','Revised'),
(7, 'Merged','Merged'),
(8, 'TechEndorsed','endorsed'),
(9, 'Withdrawn','Withdrawn'),
(10, 'Reissued','Reissued'),
(11, 'NotTreated', 'not treated')