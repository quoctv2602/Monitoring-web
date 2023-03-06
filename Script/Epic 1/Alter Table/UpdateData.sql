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
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_Data_Health]') AND type in (N'U'))
BEGIN
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Year' AND Object_ID = Object_ID(N'Trans_Data_Health'))
	BEGIN
		UPDATE Trans_Data_Health SET [Year] = DATEPART(YYYY,CreatedDate)
	END
    IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Quarter' AND Object_ID = Object_ID(N'Trans_Data_Health'))
	BEGIN
		UPDATE Trans_Data_Health SET [Quarter] = DATEPART(QQ,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Month' AND Object_ID = Object_ID(N'Trans_Data_Health'))
	BEGIN
		UPDATE Trans_Data_Health SET [Month] = DATEPART(MM,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Week' AND Object_ID = Object_ID(N'Trans_Data_Health'))
	BEGIN
		UPDATE Trans_Data_Health SET [Week] = DATEPART(WW,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'DayofYear' AND Object_ID = Object_ID(N'Trans_Data_Health'))
	BEGIN
		UPDATE Trans_Data_Health SET [DayofYear] = DATEPART(DY,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Date' AND Object_ID = Object_ID(N'Trans_Data_Health'))
	BEGIN
		UPDATE Trans_Data_Health SET [Date] = DATEPART(DD,CreatedDate)
	END
END

IF EXISTS (SELECT 1
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_Message_Log]') AND type in (N'U'))
BEGIN
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Year' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		UPDATE [Trans_Message_Log] SET [Year] = DATEPART(YYYY,CreatedDate)
	END
    IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Quarter' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		UPDATE [Trans_Message_Log] SET [Quarter] = DATEPART(QQ,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Month' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		UPDATE [Trans_Message_Log] SET [Month] = DATEPART(MM,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Week' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		UPDATE [Trans_Message_Log] SET [Week] = DATEPART(WW,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'DayofYear' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		UPDATE [Trans_Message_Log] SET [DayofYear] = DATEPART(DY,CreatedDate)
	END
	IF EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Date' AND Object_ID = Object_ID(N'Trans_Message_Log'))
	BEGIN
		UPDATE [Trans_Message_Log] SET [Date] = DATEPART(DD,CreatedDate)
	END
END

IF EXISTS (SELECT 1
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_System_Health]') AND type in (N'U'))
BEGIN
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Year' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		UPDATE [Trans_System_Health] SET [Year] = DATEPART(YYYY,CreatedDate)
	END
    IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Quarter' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		UPDATE [Trans_System_Health] SET [Quarter] = DATEPART(QQ,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Month' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		UPDATE [Trans_System_Health] SET [Month] = DATEPART(MM,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Week' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		UPDATE [Trans_System_Health] SET [Week] = DATEPART(WW,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'DayofYear' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		UPDATE [Trans_System_Health] SET [DayofYear] = DATEPART(DY,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Date' AND Object_ID = Object_ID(N'Trans_System_Health'))
	BEGIN
		UPDATE [Trans_System_Health] SET [Date] = DATEPART(DD,CreatedDate)
	END
END


IF EXISTS (SELECT 1
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_System_Health_Instance]') AND type in (N'U'))
BEGIN
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Year' AND Object_ID = Object_ID(N'Trans_System_Health_Instance'))
	BEGIN
		UPDATE [Trans_System_Health_Instance] SET [Year] = DATEPART(YYYY,CreatedDate)
	END
    IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Quarter' AND Object_ID = Object_ID(N'Trans_System_Health_Instance'))
	BEGIN
		UPDATE [Trans_System_Health_Instance] SET [Quarter] = DATEPART(QQ,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Month' AND Object_ID = Object_ID(N'Trans_System_Health_Instance'))
	BEGIN
		UPDATE [Trans_System_Health_Instance] SET [Month] = DATEPART(MM,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Week' AND Object_ID = Object_ID(N'Trans_System_Health_Instance'))
	BEGIN
		UPDATE [Trans_System_Health_Instance] SET [Week] = DATEPART(WW,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'DayofYear' AND Object_ID = Object_ID(N'Trans_System_Health_Instance'))
	BEGIN
		UPDATE [Trans_System_Health_Instance] SET [DayofYear] = DATEPART(DY,CreatedDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Date' AND Object_ID = Object_ID(N'Trans_System_Health_Instance'))
	BEGIN
		UPDATE [Trans_System_Health_Instance] SET [Date] = DATEPART(DD,CreatedDate)
		
	END
END


IF EXISTS (SELECT 1
FROM sys.objects
WHERE object_id = OBJECT_ID(N'[dbo].[Trans_System_Health_Storage]') AND type in (N'U'))
BEGIN
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Year' AND Object_ID = Object_ID(N'Trans_System_Health_Storage'))
	BEGIN
		UPDATE [Trans_System_Health_Storage] SET [Year] = DATEPART(YYYY,CreateDate)
	END
    IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Quarter' AND Object_ID = Object_ID(N'Trans_System_Health_Storage'))
	BEGIN
		UPDATE [Trans_System_Health_Storage] SET [Quarter] = DATEPART(QQ,CreateDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Month' AND Object_ID = Object_ID(N'Trans_System_Health_Storage'))
	BEGIN
		UPDATE [Trans_System_Health_Storage] SET [Month] = DATEPART(MM,CreateDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Week' AND Object_ID = Object_ID(N'Trans_System_Health_Storage'))
	BEGIN
		UPDATE [Trans_System_Health_Storage] SET [Week] = DATEPART(WW,CreateDate)
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'DayofYear' AND Object_ID = Object_ID(N'Trans_System_Health_Storage'))
	BEGIN
		UPDATE [Trans_System_Health_Storage] SET [DayofYear] = DATEPART(DY,CreateDate)
		
	END
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'Date' AND Object_ID = Object_ID(N'Trans_System_Health_Storage'))
	BEGIN
		UPDATE [Trans_System_Health_Storage] SET [Date] = DATEPART(DD,CreateDate)
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
VALUES('UpdateData', 3);
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