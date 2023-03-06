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
WHERE object_id = OBJECT_ID(N'[dbo].[Sys_Integration_API]') AND type in (N'U'))
BEGIN
    IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'NodeType' AND Object_ID = Object_ID(N'Sys_Integration_API'))
	BEGIN
		ALTER TABLE Sys_Integration_API
		ADD NodeType smallint;
	END
    IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'TransactionViolationStatus' AND Object_ID = Object_ID(N'Sys_Integration_API'))
	BEGIN
		ALTER TABLE Sys_Integration_API
		DROP COLUMN TransactionViolationStatus
	END
	IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.[COLUMNS] AS c WHERE c.COLUMN_NAME =  'NodeType' AND c.TABLE_NAME = 'Sys_Integration_API')
	BEGIN
		ALTER TABLE Sys_Integration_API
		ALTER COLUMN NodeType SMALLINT
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'ServiceList' AND Object_ID = Object_ID(N'Sys_Integration_API'))
	BEGIN
		ALTER TABLE Sys_Integration_API
		ADD ServiceList nvarchar(4000)
	END

	IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.[COLUMNS] AS c WHERE c.COLUMN_NAME =  'HealthMeasurementKey' AND c.TABLE_NAME = 'Sys_Integration_API' )
	BEGIN
		ALTER TABLE Sys_Integration_API
		ALTER COLUMN HealthMeasurementKey nvarchar(MAX)
	END
	
	IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.[COLUMNS] AS c WHERE c.COLUMN_NAME =  'Appid' AND c.TABLE_NAME = 'Sys_Integration_API' )
	BEGIN
		ALTER TABLE Sys_Integration_API
		ALTER COLUMN Appid nvarchar(MAX)
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
VALUES('dbo.Alter_Sys_Integration_API.sql', 4);
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