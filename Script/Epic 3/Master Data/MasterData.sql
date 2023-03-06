------------------------------------
----Note: DB need replace "Master data" in the script.
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

	

	DECLARE @CreatedDate DATETIME
	SET @CreatedDate=GETDATE()

	DECLARE @SuperAdminEmail NVARCHAR(150)
	SET @SuperAdminEmail='abcxyz@truecommerce.com' ---QC Edit

 
	IF(NOT EXISTS (SELECT 1 FROM Sys_UserProfile AS up WHERE up.Email = @SuperAdminEmail))
	BEGIN
	
		INSERT INTO [dbo].[Sys_UserProfile] ([Email],[IsDelete],[GroupId],[UserType],[CreatedDate])
		 VALUES
		(
			@SuperAdminEmail,
			0,
			NULL,
			1,
			@CreatedDate
		)
	IF @@ERROR <> 0 SET NOEXEC ON;
		
	END
 
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Pages] WHERE [PageId] = 1 and [PageName] = 'KPI Settings') 
	BEGIN
		INSERT INTO [dbo].[Sys_Pages] ([PageId],[PageName],[CreatedDate]) VALUES (1, N'KPI Settings', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Pages] WHERE [PageId] = 2 and [PageName] = 'Dashbooards') 
	BEGIN
		INSERT INTO [dbo].[Sys_Pages] ([PageId],[PageName],[CreatedDate]) VALUES (2, N'Dashbooards', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Pages] WHERE [PageId] = 3 and [PageName] = 'Transactions') 
	BEGIN
		INSERT INTO [dbo].[Sys_Pages] ([PageId],[PageName],[CreatedDate]) VALUES (3, N'Transactions', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Pages] WHERE [PageId] = 4 and [PageName] = 'User Permission') 
	BEGIN
		INSERT INTO [dbo].[Sys_Pages] ([PageId],[PageName],[CreatedDate]) VALUES (4, N'User Permission', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 11 and [PageId] = 1) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (11,1, N'Add Node', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 12 and [PageId] = 1) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (12,1, N'Manage Node (Edit/ Delete)', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 13 and [PageId] = 1) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (13,1, N'Set KPI', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 14 and [PageId] = 1) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (14,1, N'Update KPI', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 15 and [PageId] = 1) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (15,1, N'Enable | Disable KPI', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 16 and [PageId] = 1) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (16,1, N'Import | Export KPI', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 21 and [PageId] = 2) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (21,2, N'View System Based', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 22 and [PageId] = 2) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (22,2, N'View Transaction Based', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 31 and [PageId] = 3) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (31,3, N'View Log', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 32 and [PageId] = 3) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (32,3, N'View Data Content', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 33 and [PageId] = 3) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (33,3, N'View Config', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 34 and [PageId] = 3) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (34,3, N'Monitoring Action (Mark as Resolved|Un-resolved|Informed)', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 35 and [PageId] = 3) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (35,3, N'Notes', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 41 and [PageId] = 4) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (41,4, N'Add Group', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 42 and [PageId] = 4) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (42,4, N'Manage Group (Edit/Delete)', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 43 and [PageId] = 4) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (43,4, N'Assign Permissions', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 44 and [PageId] = 4) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (44,4, N'Set Default Group', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
	
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Pages] WHERE [PageId] = 5 and [PageName] = 'Notification Settings') 
	BEGIN
		INSERT INTO [dbo].[Sys_Pages] ([PageId],[PageName],[CreatedDate]) VALUES (5, N'Notification Settings', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 45 and [PageId] = 4) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (45,4, N'Add User', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 46 and [PageId] = 4) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (46,4, N'Manage User (Edit/Delete)', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 51 and [PageId] = 5) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (51,5, N'Add Notification', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 52 and [PageId] = 5) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (52,5, N'Manage Notification (Edit/Delete)', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
	
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Action] WHERE [ActionId] = 53 and [PageId] = 5) 
	BEGIN
		INSERT INTO [dbo].[Sys_Action] ([ActionId],[PageId],[ActionName],[CreatedDate]) VALUES (53,5, N'On/Off Notification', @CreatedDate)
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
GO
	/* End of your 1st script */

/* End of your main script */

/* Tracking HISTORY : insert script history */
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DBScriptHistory]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[DBScriptHistory](
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

INSERT INTO [dbo].[DBScriptHistory]([FileName],[Version])
VALUES('MasterData',5);
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