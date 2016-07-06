CREATE TABLE [dbo].[ChangeRequestTsgData] (
    [Pk_ChangeRequestTsgData] INT            IDENTITY (1, 1) NOT NULL,
    [TSGTdoc]                 VARCHAR (200)  NULL,
    [TSGTarget]               INT            NULL,
    [TSGMeeting]              INT            NULL,
    [TSGSourceOrganizations]  VARCHAR (1000) NULL,
    [Fk_ChangeRequest]        INT            NOT NULL,
    [Fk_TsgStatus]            INT            NULL,
    CONSTRAINT [PK_ChangeRequestTsgData] PRIMARY KEY CLUSTERED ([Pk_ChangeRequestTsgData] ASC),
    CONSTRAINT [FK_ChangeRequestTsgData_ChangeRequest] FOREIGN KEY ([Fk_ChangeRequest]) REFERENCES [dbo].[ChangeRequest] ([Pk_ChangeRequest]),
    CONSTRAINT [FK_ChangeRequestTsgData_TsgStatus] FOREIGN KEY ([Fk_TsgStatus]) REFERENCES [dbo].[Enum_ChangeRequestStatus] ([Pk_EnumChangeRequestStatus])
);












GO
CREATE NONCLUSTERED INDEX [TsgTDocUid_index]
    ON [dbo].[ChangeRequestTsgData]([TSGTdoc] ASC);


GO
CREATE NONCLUSTERED INDEX [TsgTdocIndex]
    ON [dbo].[ChangeRequestTsgData]([TSGTdoc] ASC);


GO
CREATE NONCLUSTERED INDEX [CRTsgDataIndexWithIncludedColumns]
    ON [dbo].[ChangeRequestTsgData]([Fk_ChangeRequest] ASC)
    INCLUDE([Pk_ChangeRequestTsgData], [TSGTdoc], [TSGTarget], [TSGMeeting], [TSGSourceOrganizations], [Fk_TsgStatus]);

