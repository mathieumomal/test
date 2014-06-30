﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OfflineDBSetup.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}' AND  TABLE_NAME = 'Offline_{1}'))
BEGIN
    DROP TABLE [{0}].[Offline_{1}]
END
GO

CREATE TABLE [{0}].[Offline_{1}](
	[Offline_PK_Id] [int] NOT NULL,
	[Online_PK_Id]  [int] NOT NULL,
	[Operation]     [char](1) NOT NULL,
	[SyncStatus]    [char](1) NOT NULL,
 CONSTRAINT [PK_Offline_{1}] PRIMARY KEY CLUSTERED 
(
	[Offline_PK_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO")]
        public string TrackingTable {
            get {
                return ((string)(this["TrackingTable"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"IF EXISTS (SELECT 1 FROM SYS.TRIGGERS WHERE NAME = '{1}_Insert_Trigger')
BEGIN
    DROP TRIGGER [{0}].[{1}_Insert_Trigger]
END
GO

CREATE TRIGGER [{0}].[{1}_Insert_Trigger] 
ON [{0}].[{1}] 
FOR INSERT 
AS
BEGIN
    INSERT INTO [{0}].[Offline_{1}] (Offline_Pk_Id, Online_Pk_Id, Operation, SyncStatus)
    SELECT {2}, {2}, 'I', 'N' FROM INSERTED
END
GO")]
        public string InsertTrigger {
            get {
                return ((string)(this["InsertTrigger"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"IF EXISTS (SELECT 1 FROM SYS.TRIGGERS WHERE NAME = '{1}_Update_Trigger')
BEGIN
    DROP TRIGGER [{0}].[{1}_Update_Trigger]
END
GO

CREATE TRIGGER [{0}].[{1}_Update_Trigger] 
ON [{0}].[{1}] 
FOR UPDATE 
AS
BEGIN
    DECLARE @PK_ID INT
    SELECT @PK_ID = {2} FROM INSERTED
    IF NOT EXISTS (SELECT 1 FROM [{0}].[Offline_{1}] WHERE [Offline_Pk_Id] = @PK_ID)
	BEGIN
        INSERT INTO [{0}].[Offline_{1}] (Offline_Pk_Id, Online_Pk_Id, Operation, SyncStatus)
        SELECT {2}, {2}, 'U', 'N' FROM INSERTED
	END
    ELSE IF EXISTS (SELECT 1 FROM [{0}].[Offline_{1}] WHERE [Offline_Pk_Id] = @PK_ID AND [SyncStatus] = 'Y')
    BEGIN
        UPDATE [{0}].[Offline_{1}]
        SET    [Operation] = 'U'
        ,      [SyncStatus] = 'N'
        WHERE  [Offline_Pk_Id] = @PK_ID
    END
END
GO
")]
        public string UpdateTrigger {
            get {
                return ((string)(this["UpdateTrigger"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"IF EXISTS (SELECT 1 FROM SYS.TRIGGERS WHERE NAME = '{1}_Delete_Trigger')
BEGIN
    DROP TRIGGER [{0}].[{1}_Delete_Trigger]
END
GO

CREATE TRIGGER [{0}].[{1}_Delete_Trigger] 
ON [{0}].[{1}] 
FOR DELETE 
AS
BEGIN
	DECLARE @PK_ID INT
	SELECT @PK_ID = {2} FROM DELETED
    IF NOT EXISTS (SELECT 1 FROM [{0}].[Offline_{1}] WHERE [Offline_Pk_Id] = @PK_ID)
	BEGIN
        INSERT INTO [{0}].[Offline_{1}] (Offline_Pk_Id, Online_Pk_Id, Operation, SyncStatus)
        SELECT {2}, {2}, 'D', 'N' FROM DELETED
	END
	ELSE
	BEGIN
	    IF EXISTS (SELECT 1 FROM [{0}].[Offline_{1}] WHERE [Offline_Pk_Id] = @PK_ID AND [Operation] = 'I' AND [SyncStatus] = 'N')
		BEGIN
		   DELETE FROM [{0}].[Offline_{1}] WHERE Offline_Pk_Id = @PK_ID
		END
		ELSE
		BEGIN
		   UPDATE {0}.Offline_{1}
		   SET    [Operation] = 'D'
           ,      [SyncStatus] = 'N'
		   WHERE  Offline_Pk_Id = @PK_ID
		END
	END
END
GO
")]
        public string DeleteTrigger {
            get {
                return ((string)(this["DeleteTrigger"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"IF EXISTS (SELECT 1 FROM SYS.OBJECTS WHERE TYPE = 'P' AND NAME = 'Offline_{1}_GetChanges')
BEGIN
    DROP PROCEDURE [{0}].[Offline_{1}_GetChanges]
END
GO

CREATE PROCEDURE [{0}].[Offline_{1}_GetChanges]
	@Operation Char(1)
AS
BEGIN
	IF (@Operation = 'D')
	BEGIN
	    SELECT     [OnlinePrimaryKey]   = [tracking].[Online_PK_Id]
        ,          [OfflinePrimaryKey]  = [tracking].[Offline_PK_Id]
        FROM       {0}.Offline_{1} [tracking]
        WHERE      [tracking].[Operation] = @Operation
        AND        [tracking].[SyncStatus] = 'N' 
	END
	ELSE
	BEGIN
	    SELECT     [OnlinePrimaryKey]   = [tracking].[Online_PK_Id]
        ,          [OfflinePrimaryKey]  = [tracking].[Offline_PK_Id]
{3}{4}        FROM       {0}.{1} [base]
        INNER JOIN {0}.Offline_{1} [tracking] ON [base].{2} = [tracking].Offline_Pk_Id
{5}        WHERE      [tracking].[Operation] = @Operation
        AND        [tracking].[SyncStatus] = 'N'
    END 
END
GO")]
        public string GetChangesStoredProcedure {
            get {
                return ((string)(this["GetChangesStoredProcedure"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("        ,          [{0}] = [base].[{0}]")]
        public string ColumnSQL {
            get {
                return ((string)(this["ColumnSQL"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("        ,          [{0}]   = ISNULL([{1}].Online_Pk_Id, [base].{0})")]
        public string ForeignKeyColumnSQL {
            get {
                return ((string)(this["ForeignKeyColumnSQL"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("        LEFT  JOIN {0}.Offline_{1} [{2}]         ON [base].{3} = [{2}].Offline_Pk" +
            "_Id")]
        public string ForeignKeyTableSQL {
            get {
                return ((string)(this["ForeignKeyTableSQL"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=(local);Initial Catalog=U3GPPDB;Integrated Security=True")]
        public string ConnectionString {
            get {
                return ((string)(this["ConnectionString"]));
            }
            set {
                this["ConnectionString"] = value;
            }
        }
    }
}