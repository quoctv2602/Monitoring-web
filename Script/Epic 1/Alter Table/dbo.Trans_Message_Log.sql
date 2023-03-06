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

IF EXISTS (SELECT 1
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_Message_Log]') AND type in (N'U'))
BEGIN
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Year' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		ALTER TABLE [Trans_Message_Log]
		ADD [Year] int
		ALTER TABLE [dbo].[Trans_Message_Log] ADD  CONSTRAINT [DF_Trans_Message_Log_Year]  DEFAULT (DATEPART(YYYY, GETDATE())) FOR [Year]
	END
    IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Quarter' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		ALTER TABLE Trans_Message_Log
		ADD [Quarter] int
		ALTER TABLE [dbo].[Trans_Message_Log] ADD  CONSTRAINT [DF_Trans_Message_Log_Quarter]  DEFAULT (DATEPART(QQ, GETDATE())) FOR [Quarter]
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Month' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		ALTER TABLE Trans_Message_Log
		ADD [Month] int
		ALTER TABLE [dbo].[Trans_Message_Log] ADD  CONSTRAINT [DF_Trans_Message_Log_Month]  DEFAULT (DATEPART(MM, GETDATE())) FOR [Month]
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Week' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		ALTER TABLE Trans_Message_Log
		ADD [Week] int
		ALTER TABLE [dbo].[Trans_Message_Log] ADD  CONSTRAINT [DF_Trans_Message_Log_Week]  DEFAULT (DATEPART(WW, GETDATE())) FOR [Week]
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'DayofYear' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		ALTER TABLE Trans_Message_Log
		ADD [DayofYear] int
		ALTER TABLE [dbo].[Trans_Message_Log] ADD  CONSTRAINT [DF_Trans_Message_Log_DayofYear]  DEFAULT (DATEPART(DY, GETDATE())) FOR [DayofYear]
	END
	IF Not EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Date' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		ALTER TABLE Trans_Message_Log
		ADD [Date] int
		ALTER TABLE [dbo].[Trans_Message_Log] ADD  CONSTRAINT [DF_Trans_Message_Log_Date]  DEFAULT (DATEPART(DD, GETDATE())) FOR [Date]
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
VALUES('Trans_Message_Log', 3);
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