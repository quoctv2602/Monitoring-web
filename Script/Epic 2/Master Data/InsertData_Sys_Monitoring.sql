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
WHERE object_id = OBJECT_ID(N'[dbo].[Sys_Monitoring]') AND type in (N'U'))
BEGIN
	
	IF  EXISTS(SELECT 1 FROM sys.columns  WHERE Name = N'NodeType' AND Object_ID = Object_ID(N'Sys_Monitoring'))
	BEGIN
		UPDATE Sys_Monitoring SET [NodeType] = 2 WHERE ID <= 6
	END
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 7 and [Name] = 'Failed Transaction') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit],[Orders],[NodeType]) VALUES (7, N'Failed Transaction', N'Transactions',7,1)
	END
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 8 and [Name] = 'Intergration Error Transaction') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit],[Orders],[NodeType]) VALUES (8, N'Intergration Error Transaction', N'Transactions',8,1)
	END
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 9 and [Name] = 'Pending Transaction') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit],[Orders],[NodeType]) VALUES (9, N'Pending Transaction', N'Transactions',9,1)
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
VALUES('InsertData_Sys_Monitoring.sql', 1);
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