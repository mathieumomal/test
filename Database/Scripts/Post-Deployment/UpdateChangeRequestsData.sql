DELETE FROM ChangeRequestTsgData
GO

INSERT INTO ChangeRequestTsgData
	SELECT TSGTDoc, TSGTarget, TSGMeeting, TSGSourceOrganizations, Pk_ChangeRequest as Fk_ChangeRequest, Fk_TSGStatus as Fk_TsgStatus
	FROM ChangeRequest
	WHERE TSGTDoc IS NOT NULL AND TSGTDoc <> ''
GO