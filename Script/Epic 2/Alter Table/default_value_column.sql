------------------------------------
----Note: DB need replace "DB Name Change" in the script.
----1. [Monitoring]
------------------------------------
/* Begin SCRIPTS - Begin Transaction */
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT ON
GO
USE [Monitoring]
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL Serializable
GO
BEGIN TRANSACTION
GO
/* Begin SCRIPTS*/
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_Message_Log]') AND type in (N'U'))
BEGIN
	IF(NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS AS c WHERE c.TABLE_NAME = 'Trans_Message_Log' AND c.COLUMN_NAME = 'CreatedDate' AND c.COLUMN_DEFAULT IS not null))
	BEGIN
		ALTER TABLE [dbo].[Trans_Message_Log] ADD  CONSTRAINT [DF_Trans_Message_Log_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
	END
END
 


IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_Data_Health]') AND type in (N'U'))
BEGIN
	IF(NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS AS c WHERE c.TABLE_NAME = 'Trans_Data_Health' AND c.COLUMN_NAME = 'CreatedDate' AND c.COLUMN_DEFAULT IS not null))
	BEGIN
		ALTER TABLE [dbo].[Trans_Data_Health] ADD  CONSTRAINT [DF_Trans_Data_Health_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
	END
END

IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_System_Health_Instance]') AND type in (N'U'))
BEGIN
	IF(NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS AS c WHERE c.TABLE_NAME = 'Trans_System_Health_Instance' AND c.COLUMN_NAME = 'CreatedDate' AND c.COLUMN_DEFAULT IS not null))
	BEGIN
		ALTER TABLE [dbo].[Trans_System_Health_Instance] ADD  CONSTRAINT [DF_Trans_System_Health_Instance_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
	END
END


IF @@ERROR <> 0 SET NOEXEC ON
	GO
/* End of your 1st script */

/* End of your main script */

/* Tracking HISTORY : insert script history */
IF NOT EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[DBScriptHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DBScriptHistory]
    (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [FileName] [varchar](50) NOT NULL,
        [Version] [int] NOT NULL,
        [ExcuteDate] [datetime] NOT NULL,
        CONSTRAINT [PK_DBScriptHistory] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    ALTER TABLE [dbo].[DBScriptHistory] ADD  CONSTRAINT [DF_DBScriptHistory_ExcuteDate]  DEFAULT (getdate()) FOR [ExcuteDate]

END

INSERT INTO [dbo].[DBScriptHistory]
    ([FileName],[Version])
VALUES('default_value_column.sql', 1);
GO
/* Tracking HISTORY */

/* End SCRIPTS - COMMIT TRANSACTION */
COMMIT TRANSACTION
GO
IF @@ERROR <> 0 SET NOEXEC ON
GO
DECLARE @Success AS BIT
SET @Success = 1
SET NOEXEC OFF
IF (@Success = 1) PRINT 'The database update succeeded'
ELSE BEGIN
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
    PRINT 'The database update failed'
END
GO
/* End SCRIPTS - COMMIT TRANSACTION */