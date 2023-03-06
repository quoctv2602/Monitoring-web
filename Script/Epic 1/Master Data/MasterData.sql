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


DECLARE @EnvironmentID INT
DECLARE @EnvironmentName NVARCHAR(125)
DECLARE @EnvironmentCommnet NVARCHAR(125)

SET @EnvironmentName = N'TAN2' -- QCs Team edit Name
SET @EnvironmentCommnet = N'QC Comment' -- QCs Team edit Comment


DECLARE @FromEmail VARCHAR(125)
SET @FromEmail = 'no-reply-cip@dicentral.com' -- QC Edit

DECLARE @SmtpServer VARCHAR(125) 
SET @SmtpServer = 'smtp.office365.com' -- QC Edit

DECLARE @Port INT
SET @Port = 587 -- QC Edit

DECLARE @MailUserName VARCHAR(125) 
SET @MailUserName = 'no-reply-cip@dicentral.com' -- QC Edit

DECLARE @PassWordMail VARCHAR(125)
SET @PassWordMail = '@qwa!BHN#139'  -- QC Edit

 
IF(NOT EXISTS (SELECT 1 FROM Sys_Environment AS se WHERE se.Name = @EnvironmentName))
BEGIN
	


	INSERT INTO Sys_Environment
	(
		-- ID -- this column value is auto-generated
		Name,
		Comment
	)
	VALUES
	(
		 @EnvironmentName,@EnvironmentCommnet
	)
	SET @EnvironmentID = @@IDENTITY
	IF @@ERROR <> 0 SET NOEXEC ON;
	
 
	INSERT INTO Sys_EmailServer
	(
		 ID,
		FromEmail,
		SmtpServer,
		Port,
		UserName,
		[Password],
		DisplayName,
		EnableSSL,
		EnvironmentId,
		Comment
	)
	VALUES
	(
		@@IDENTITY,
		 @FromEmail,
		 @SmtpServer,
		 @Port,
		 @MailUserName,
		 @PassWordMail,
		 'Monitoring System',
		 1,
		 @EnvironmentID,
		 null
	 
	) 
	IF @@ERROR <> 0 SET NOEXEC ON;

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 1 and [Name] = 'CPU') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit]) VALUES (1, N'CPU', N'%')
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
		
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 2 and [Name] = 'RAM') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit]) VALUES (2, N'RAM', N'%')
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
		
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 3 and [Name] = 'Storage') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit]) VALUES (3, N'Storage', N'%')
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
		
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 4 and [Name] = 'Process Time EDItoASCII') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit]) VALUES (4, N'Process Time EDItoASCII', N'Miliseconds')
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
		
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 5 and [Name] = 'Free Disk') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit]) VALUES (5, N'Free Disk', N'%')
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
		
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Sys_Monitoring] WHERE [ID] = 6 and [Name] = 'Shared Storage Running Time') 
	BEGIN
		INSERT [dbo].[Sys_Monitoring] ([ID], [Name], [Unit]) VALUES (6, N'Shared Storage Running Time', N'Miliseconds')
	END
	IF @@ERROR <> 0 SET NOEXEC ON;
		
	end
 
IF @@ERROR <> 0 SET NOEXEC ON
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
VALUES('MasterData',2);
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