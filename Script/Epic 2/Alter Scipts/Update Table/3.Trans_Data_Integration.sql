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
IF NOT EXISTS (SELECT 1
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_Data_Integration]') AND type in (N'U'))
BEGIN

    CREATE TABLE [dbo].[Trans_Data_Integration]
    (
        [ID] [uniqueidentifier] NOT NULL,
        [EnviromentID] [int] NULL,
        [Note] [nvarchar](max) NULL,
        [TransactionKey] [uniqueidentifier] NULL,
        [MonitoredStatus] [int] NULL,
        [ReProcess] [smallint] NULL,
        [UpdateDate] [datetime] NULL,
        [UpdateBy] [nvarchar](128) NULL,
        [CreateDate] [datetime] NULL,
        [Year] [int] NULL,
        [Quarter] [int] NULL,
        [Month] [int] NULL,
        [Week] [int] NULL,
        [DayofYear] [int] NULL,
        [Date] [int] NULL,
        CONSTRAINT [PK_Trans_Data_Integration] PRIMARY KEY CLUSTERED 
    (
    [ID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    ALTER TABLE [dbo].[Trans_Data_Integration] ADD  CONSTRAINT [DF_Trans_Data_Integration_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
    ALTER TABLE [dbo].[Trans_Data_Integration] ADD  CONSTRAINT [DF_Trans_Data_Integration_Year]  DEFAULT (datepart(year,getdate())) FOR [Year]
    ALTER TABLE [dbo].[Trans_Data_Integration] ADD  CONSTRAINT [DF_Trans_Data_Integration_Quarter]  DEFAULT (datepart(quarter,getdate())) FOR [Quarter]
    ALTER TABLE [dbo].[Trans_Data_Integration] ADD  CONSTRAINT [DF_Trans_Data_Integration_Month]  DEFAULT (datepart(month,getdate())) FOR [Month]
    ALTER TABLE [dbo].[Trans_Data_Integration] ADD  CONSTRAINT [DF_Trans_Data_Integration_Week]  DEFAULT (datepart(week,getdate())) FOR [Week]
    ALTER TABLE [dbo].[Trans_Data_Integration] ADD  CONSTRAINT [DF_Trans_Data_Integration_DayofYear]  DEFAULT (datepart(dayofyear,getdate())) FOR [DayofYear]
    ALTER TABLE [dbo].[Trans_Data_Integration] ADD  CONSTRAINT [DF_Trans_Data_Integration_Date]  DEFAULT (datepart(day,getdate())) FOR [Date]

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
VALUES('3.Trans_Data_Integration.sql', 1);
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