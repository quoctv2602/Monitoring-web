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
WHERE object_id = OBJECT_ID(N'[dbo].[TransactionBase]') AND type in (N'U'))
BEGIN
    
	CREATE TABLE [dbo].[TransactionBase](
	[ID] [uniqueidentifier] NOT NULL,
	[RequestID] [uniqueidentifier] NULL,
	[EnvironmentID] [int] NULL,
	[RequestTime] [datetime] NULL,
	[ResponseTime] [datetime] NULL,
	[CIPFolow] [char](1) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[ContentData] [nvarchar](max) NULL,
	[ErrorNumbers] [int] NULL,
	[IntergrationErrorNumbers] [int] NULL,
	[Status] [int] NULL,
	[Error_Message] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[Year] [int] NULL,
	[Quarter] [int] NULL,
	[Month] [int] NULL,
	[Week] [int] NULL,
	[DayofYear] [int] NULL,
	[Date] [int] NULL,
	CONSTRAINT [PK_TransactionBase] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_ID]  DEFAULT (newid()) FOR [ID]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_Status]  DEFAULT ((0)) FOR [Status]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_Year]  DEFAULT (datepart(year,getdate())) FOR [Year]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_Quarter]  DEFAULT (datepart(quarter,getdate())) FOR [Quarter]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_Month]  DEFAULT (datepart(month,getdate())) FOR [Month]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_Week]  DEFAULT (datepart(week,getdate())) FOR [Week]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_DayofYear]  DEFAULT (datepart(dayofyear,getdate())) FOR [DayofYear]

	ALTER TABLE [dbo].[TransactionBase] ADD  CONSTRAINT [DF_TransactionBase_Date]  DEFAULT (datepart(day,getdate())) FOR [Date]
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
VALUES('TransactionBase.sql', 1);
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