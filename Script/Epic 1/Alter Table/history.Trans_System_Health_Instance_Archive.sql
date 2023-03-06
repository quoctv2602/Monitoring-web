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
IF NOT EXISTS (SELECT *
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[history].[Trans_System_Health_Instance_Archive]') AND type in (N'U'))
	BEGIN
			CREATE TABLE [history].[Trans_System_Health_Instance_Archive](
		[ID] [uniqueidentifier] NOT NULL,
		[RequestID] [uniqueidentifier] NULL,
		[EnvironmentID] [int] NULL,
		[MachineName] [nvarchar](500) NULL,
		[IPAddress] [nvarchar](125) NULL,
		[ProcessName] [nvarchar](500) NULL,
		[CPU] [int] NULL,
		[Ram] [int] NULL,
		[Storage] [int] NULL,
		[Instance] [int] NULL,
		[Status] [nvarchar](50) NULL,
		[CreatedDate] [datetime] NULL,
		[Year] [int] NULL,
		[Quarter] [int] NULL,
		[Month] [int] NULL,
		[Week] [int] NULL,
		[DayofYear] [int] NULL,
		[Date] [int] NULL,
	 CONSTRAINT [PK_Trans_System_Health_Instance_Archive] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
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
VALUES('history.Trans_System_Health_Instance_Archive.sql', 3);
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