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
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_System_Health]') AND type in (N'U'))
BEGIN
    IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'FreeDiskViolationStatus' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		ALTER TABLE Trans_System_Health
		ADD FreeDiskViolationStatus int;
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'TransactionViolationStatus' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		ALTER TABLE Trans_System_Health
		ADD TransactionViolationStatus int;
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'FileTransferViolationStatus' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		ALTER TABLE Trans_System_Health
		ADD FileTransferViolationStatus int;
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'ServiceViolation' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		ALTER TABLE Trans_System_Health
		ADD ServiceViolation bit;
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'ServiceViolationStatus' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		ALTER TABLE Trans_System_Health
		ADD ServiceViolationStatus int;
	END


END
IF EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Sys_Monitoring]') AND type in (N'U'))
BEGIN
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Orders' AND Object_ID = Object_ID(N'Sys_Monitoring'))
	BEGIN
		ALTER TABLE Sys_Monitoring
		ADD [Orders] INT;
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
VALUES('dbo.Alter_Trans_System_Health.sql', 2);
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