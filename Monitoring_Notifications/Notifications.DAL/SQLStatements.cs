using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL
{
    public class SQLStatements
    {
        public static string GetEmailServers = @"SELECT ID, FromEmail, SmtpServer, 
                Port, UserName, Password, EnableSSL, EnvironmentId, DisplayName, Comment 
                FROM Sys_EmailServer WITH (NOLOCK) WHERE EnvironmentId = {0} ";
        
        public static string GetNodeSettings = @"SELECT ID, NodeName, EnvironmentID, MachineName, 
                Description, ServiceList, NotificationEmail, ReportEmail, CreateDate , 
                NotificationAlias, ReportAlias 
                FROM Sys_Node_Setting  WITH (NOLOCK) WHERE   IsActive = 1 AND  NodeType = 2 AND EnvironmentID = {0} AND MachineName = '{1}' ";
        
        public static string GetNodeTransactionSettings = @"SELECT ID, isnull(NodeName,'') NodeName, EnvironmentID, isnull(MachineName,'') MachineName, 
                Description, ServiceList, NotificationEmail, ReportEmail, CreateDate , 
                NotificationAlias, ReportAlias 
                FROM Sys_Node_Setting  WITH (NOLOCK) WHERE   IsActive = 1 AND  NodeType = 1  AND EnvironmentID = {0} ";


        public static string GetNodeSettingsByNodeType = @"SELECT ID,isnull( NodeName,'') NodeName, EnvironmentID, isnull(MachineName,'') MachineName, 
                Description, ServiceList, NotificationEmail, ReportEmail, CreateDate , 
                NotificationAlias, ReportAlias 
                FROM Sys_Node_Setting  WITH (NOLOCK) WHERE EnvironmentID = {0}  AND NodeType = {1} ";

        public static string GetNodeList = @"
            SELECT DISTINCT ID, ISNULL(NodeName,'') NodeName, EnvironmentID,ISNULL( MachineName,'' ) MachineName, 
            Description, ServiceList, NotificationEmail, ReportEmail, CreateDate, 
            NotificationAlias, ReportAlias 
            FROM Sys_Node_Setting WITH (NOLOCK)
            WHERE IsActive = 1
            AND NodeType = 2
                ";

        public static string GetEnvironments = @"SELECT ID, Name, Comment 
                            FROM Sys_Environment WITH (NOLOCK) WHERE ID = {0} ";

        public static string GetMonitorings = @"SELECT ID, Name, Unit 
                            FROM Sys_Monitoring WITH (NOLOCK) WHERE ID = {0} ";

        public static string GetListMailNotification = @"SELECT snd.ID,snd.Emails,snd.NotificationAlias
                                                        FROM Sys_Notification AS sn  WITH (NOLOCK) 
                                                        LEFT JOIN Sys_Notification_Detail AS snd  WITH (NOLOCK)  ON snd.NotificationId = sn.ID
                                                        WHERE sn.IsActive = 1
                                                        AND snd.KPI = {0}";


        public static string GetThresholdRules = @"SELECT r.ID, r.Node_Setting, r.EnvironmentID,isnull( r.MachineName,'') MachineName,
                                                    r.MonitoringType, r.MonitoringName, r.Condition, r.Threshold, r.ThresholdCounter, r.CreateDate
                                                    FROM Sys_Threshold_Rule AS r WITH (NOLOCK)  
                                                    LEFT JOIN Sys_Node_Setting AS sns WITH (NOLOCK)  ON sns.ID = r.Node_Setting
                                                    WHERE MonitoringType = {0}
                                                    AND sns.IsActive  =1 ";

        public static string GetAndUpdateViolatedRecordsByErrorNumbers = @"
        DECLARE @Result TABLE
        (
	        Id uniqueidentifier,
	        EnvironmentId int,
	        ErrorNumbersViolation int,
	        ErrorNumbersViolationStatus int,
	        DenseRankresult int,
            CreatedDate DateTime
        )
        ;WITH CTE AS (
	        SELECT tsh.Id, tsh.EnvironmentId,  tsh.ErrorNumbersViolation, tsh.Sequence_Id, tsh.ErrorNumbersViolationStatus, CreatedDate, 
		        tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId ORDER BY tsh.Sequence_Id) as DenseRankresult
	        FROM (
		        SELECT Id, tb.EnvironmentId, tb.ErrorNumbersViolation, tb.ErrorNumbersViolationStatus, tb.CreatedDate, 
		        ROW_NUMBER() OVER (PARTITION BY tb.EnvironmentId  ORDER BY tb.CreatedDate) AS Sequence_Id
		        FROM Trans_Data_Summary  AS tb
		        WHERE CONVERT(DATE,tb.CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
	        ) tsh 
	        WHERE tsh.ErrorNumbersViolation = 1 
	        AND {0}
	        AND tsh.EnvironmentId = {1}
        )
                

				

        INSERT INTO @Result(id, EnvironmentId, ErrorNumbersViolation, ErrorNumbersViolationStatus, DenseRankresult, CreatedDate )
        SELECT id, EnvironmentId, ErrorNumbersViolation, ErrorNumbersViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
        UPDATE a SET 
	        ErrorNumbersViolationStatus =  {3}
	        OUTPUT INSERTED.EnvironmentId, '' MachineName,  INSERTED.Id
        FROM Trans_Data_Summary  a
        WHERE a.Id IN 
            (SELECT TOP({2}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
		        WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {2})
			        ) d  ORDER by d.createdDate)
        ";
        public static string GetAndUpdateViolatedRecordsByIntergrationErrorNumbers = @"
        DECLARE @Result TABLE
        (
	        Id uniqueidentifier,
	        EnvironmentId int,
	        IntergrationErrorNumbersViolation int,
	        IntergrationErrorNumbersViolationStatus int,
	        DenseRankresult int,
            CreatedDate DateTime
        )
        ;WITH CTE AS (
	        SELECT tsh.Id, tsh.EnvironmentId,  tsh.IntergrationErrorNumbersViolation, tsh.Sequence_Id, tsh.IntergrationErrorNumbersViolationStatus, CreatedDate, 
		        tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId ORDER BY tsh.Sequence_Id) as DenseRankresult
	        FROM (
		        SELECT Id, tb.EnvironmentId, tb.IntergrationErrorNumbersViolation, tb.IntergrationErrorNumbersViolationStatus, tb.CreatedDate, 
		        ROW_NUMBER() OVER (PARTITION BY tb.EnvironmentId  ORDER BY tb.CreatedDate) AS Sequence_Id
		        FROM Trans_Data_Summary  AS tb
		        WHERE CONVERT(DATE,tb.CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
	        ) tsh 
	        WHERE tsh.IntergrationErrorNumbersViolation = 1 
	        AND {0}
	        AND tsh.EnvironmentId = {1}
        )
                

				

        INSERT INTO @Result(id, EnvironmentId, IntergrationErrorNumbersViolation, IntergrationErrorNumbersViolationStatus, DenseRankresult, CreatedDate )
        SELECT id, EnvironmentId, IntergrationErrorNumbersViolation, IntergrationErrorNumbersViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
        UPDATE a SET 
	        IntergrationErrorNumbersViolationStatus =  {3}
	        OUTPUT INSERTED.EnvironmentId,'' MachineName,   INSERTED.Id
        FROM Trans_Data_Summary  a
        WHERE a.Id IN 
            (SELECT TOP({2}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
		        WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {2})
			        ) d  ORDER by d.createdDate)
        ";
        public static string GetAndUpdateViolatedRecordsByPendingTransactions = @"
        DECLARE @Result TABLE
        (
	        Id uniqueidentifier,
	        EnvironmentId int,
	        PendingTransactionsViolation int,
	        PendingTransactionsViolationStatus int,
	        DenseRankresult int,
            CreatedDate DateTime
        )
        ;WITH CTE AS (
	        SELECT tsh.Id, tsh.EnvironmentId,  tsh.PendingTransactionsViolation, tsh.Sequence_Id, tsh.PendingTransactionsViolationStatus, CreatedDate, 
		        tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId ORDER BY tsh.Sequence_Id) as DenseRankresult
	        FROM (
		        SELECT Id, tb.EnvironmentId, tb.PendingTransactionsViolation, tb.PendingTransactionsViolationStatus, tb.CreatedDate, 
		        ROW_NUMBER() OVER (PARTITION BY tb.EnvironmentId  ORDER BY tb.CreatedDate) AS Sequence_Id
		        FROM Trans_Data_Summary  AS tb
		        WHERE CONVERT(DATE,tb.CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
	        ) tsh 
	        WHERE tsh.PendingTransactionsViolation = 1 
	        AND {0}
	        AND tsh.EnvironmentId = {1}
        )
                

				

        INSERT INTO @Result(id, EnvironmentId, PendingTransactionsViolation, PendingTransactionsViolationStatus, DenseRankresult, CreatedDate )
        SELECT id, EnvironmentId, PendingTransactionsViolation, PendingTransactionsViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
        UPDATE a SET 
	        PendingTransactionsViolationStatus =  {3}
	        OUTPUT INSERTED.EnvironmentId,'' MachineName,   INSERTED.Id
        FROM Trans_Data_Summary  a
        WHERE a.Id IN 
            (SELECT TOP({2}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
		        WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {2})
			        ) d  ORDER by d.createdDate)    
        ";

         
        public static string GetAndUpdateViolatedRecordsByCPU= @"
                DECLARE @Result TABLE
                (
	                Id uniqueidentifier,
	                EnvironmentId int,
	                MachineName nvarchar(255),
	                CPUViolation int,
	                CPUViolationStatus int,
	                DenseRankresult int,
                    CreatedDate DateTime
                )
                ;WITH CTE AS (
                SELECT tsh.Id, tsh.EnvironmentId, tsh.MachineName, tsh.CPUViolation, tsh.Sequence_Id, tsh.CPUViolationStatus, CreatedDate, 
	                tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId, tsh.MachineName ORDER BY tsh.Sequence_Id) as DenseRankresult
                FROM (SELECT Id, EnvironmentId, MachineName, CPUViolation, CPUViolationStatus, CreatedDate, 
			                ROW_NUMBER() OVER (PARTITION BY EnvironmentId, MachineName  ORDER BY CreatedDate) AS Sequence_Id
		                FROM Trans_System_Health
                        WHERE CONVERT(DATE, CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
                            --CreatedDate BETWEEN DATEADD(hh,{0},DATEADD(dd,DATEDIFF(dd,0,GETDATE()-1),0)) AND GETDATE()
                        ) tsh 
                WHERE CPUViolation = 1 AND {1} AND EnvironmentId = {2} AND MachineName = '{3}')
                
                INSERT INTO @Result(id, EnvironmentId,MachineName, CPUViolation, CPUViolationStatus, DenseRankresult, CreatedDate )
                SELECT id, EnvironmentId,MachineName, CPUViolation, CPUViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
                UPDATE a SET 
	                CPUViolationStatus =  {5}
	                OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName, INSERTED.Id
                FROM Trans_System_Health a
                WHERE a.Id IN 
                    (SELECT TOP({4}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
						WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {4})
							) d  ORDER by d.createdDate)
                "; // for 3 and 6 records
        public static string GetAndUpdateViolatedRecordsByFreeDisk = @"
                DECLARE @Result TABLE
                (
	                Id uniqueidentifier,
	                EnvironmentId int,
	                MachineName nvarchar(255),
	                FreeDiskViolation int,
	                FreeDiskViolationStatus int,
	                DenseRankresult int,
                    CreatedDate DateTime
                )
                ;WITH CTE AS (
                SELECT tsh.Id, tsh.EnvironmentId, tsh.MachineName, tsh.Violation, tsh.Sequence_Id, tsh.FreeDiskViolationStatus, CreatedDate, 
	                tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId, tsh.MachineName ORDER BY tsh.Sequence_Id) as DenseRankresult
                FROM (SELECT r.Id, r.EnvironmentId, r.MachineName, d.Violation, r.FreeDiskViolationStatus, r.CreatedDate, 
			                ROW_NUMBER() OVER (PARTITION BY r.EnvironmentId, r.MachineName  ORDER BY r.CreatedDate) AS Sequence_Id
		                  FROM Trans_System_Health r
		                LEFT JOIN  Trans_System_Health_Storage AS d ON d.RequestID = r.RequestID
                        WHERE CONVERT(DATE, CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
                        ) tsh 
                WHERE Violation = 1 AND {1} AND EnvironmentId = {2} AND MachineName = '{3}')
                 
               INSERT INTO @Result(id, EnvironmentId,MachineName, FreeDiskViolation, FreeDiskViolationStatus, DenseRankresult, CreatedDate )
                SELECT id, EnvironmentId,MachineName, Violation, FreeDiskViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
                UPDATE a SET 
	                FreeDiskViolationStatus =  {5}
	                OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName, INSERTED.Id
                FROM Trans_System_Health a
                WHERE a.Id IN 
                    (SELECT TOP({4}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
						WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {4})
							) d  ORDER by d.createdDate)
                "; // for 3 and 6 records
        public static string GetAndUpdateViolatedRecordsByTransaction = @"
                DECLARE @Result TABLE
                (
	               Id uniqueidentifier,
	                EnvironmentId int,
	                MachineName nvarchar(255),
	                TransactionViolation int,
	               TransactionViolationStatus int,
	                DenseRankresult int,
                    CreatedDate DateTime
                )
                ;WITH CTE AS (
                  SELECT tsh.Id, tsh.EnvironmentId, tsh.MachineName, tsh.TransactionViolation, tsh.Sequence_Id, tsh.TransactionViolationStatus, CreatedDate, 
	                tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId, tsh.MachineName ORDER BY tsh.Sequence_Id) as DenseRankresult
                FROM (SELECT r.Id, r.EnvironmentId, r.MachineName, d.TransactionViolation, r.TransactionViolationStatus, r.CreatedDate, 
			                ROW_NUMBER() OVER (PARTITION BY r.EnvironmentId, r.MachineName  ORDER BY r.CreatedDate) AS Sequence_Id
		                  FROM Trans_System_Health r
		                LEFT JOIN Trans_Data_Health  AS d ON d.RequestID = r.RequestID
                        WHERE CONVERT(DATE, r.CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
                        ) tsh 
                WHERE TransactionViolation = 1 AND {1} AND EnvironmentId = {2} AND MachineName = '{3}')
                 
                 INSERT INTO @Result(id, EnvironmentId,MachineName, TransactionViolation, TransactionViolationStatus, DenseRankresult, CreatedDate )
                SELECT id, EnvironmentId,MachineName, TransactionViolation, TransactionViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
                UPDATE a SET 
	                TransactionViolationStatus =  {5}
	                OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName, INSERTED.Id
                FROM Trans_System_Health a
                WHERE a.Id IN 
                    (SELECT TOP({4}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
						WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {4})
							) d  ORDER by d.createdDate)
                "; // for 3 and 6 records

        public static string GetAndUpdateViolatedRecordsByFileTransfer = @"
                DECLARE @Result TABLE
                (
                    Id uniqueidentifier,
                    EnvironmentId int,
                    MachineName nvarchar(255),
                    FileTransferViolation int,
                    FileTransferViolationStatus int,
                    DenseRankresult int,
                    CreatedDate DateTime
                )
                ;WITH CTE AS (
                    SELECT tsh.Id, tsh.EnvironmentId, tsh.MachineName, tsh.FileTransferViolation, tsh.Sequence_Id, tsh.FileTransferViolationStatus, CreatedDate, 
	                tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId, tsh.MachineName ORDER BY tsh.Sequence_Id) as DenseRankresult
                FROM (SELECT r.Id, r.EnvironmentId, r.MachineName, d.FileTransferViolation, r.FileTransferViolationStatus, r.CreatedDate, 
			                ROW_NUMBER() OVER (PARTITION BY r.EnvironmentId, r.MachineName  ORDER BY r.CreatedDate) AS Sequence_Id
		                  FROM Trans_System_Health r
		                LEFT JOIN Trans_Data_Health  AS d ON d.RequestID = r.RequestID
                        WHERE CONVERT(DATE, r.CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
                        ) tsh 
                WHERE FileTransferViolation = 1 AND {1} AND EnvironmentId = {2} AND MachineName = '{3}')
                 
       INSERT INTO @Result(id, EnvironmentId,MachineName, FileTransferViolation, FileTransferViolationStatus, DenseRankresult, CreatedDate )
                SELECT id, EnvironmentId,MachineName, FileTransferViolation, FileTransferViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
                UPDATE a SET 
	                FileTransferViolationStatus =  {5}
	                OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName, INSERTED.Id
                FROM Trans_System_Health a
                WHERE a.Id IN 
                    (SELECT TOP({4}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
						WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {4})
							) d  ORDER by d.createdDate)
                "; // for 3 and 6 records

        public static string GetAndUpdateViolatedRecordsByMemory = @"
             DECLARE @Result TABLE
                (
	                Id uniqueidentifier,
	                EnvironmentId int,
	                MachineName nvarchar(255),
	                MemoryViolation int,
	                MemoryViolationStatus int,
	                DenseRankresult int,
                    CreatedDate DateTime
                )
                ;WITH CTE AS (
                SELECT tsh.Id, tsh.EnvironmentId, tsh.MachineName, tsh.MemoryViolation, tsh.Sequence_Id, tsh.MemoryViolationStatus, CreatedDate, 
	                tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId, tsh.MachineName ORDER BY tsh.Sequence_Id) as DenseRankresult
                FROM (SELECT Id, EnvironmentId, MachineName, MemoryViolation, MemoryViolationStatus, CreatedDate, 
			                ROW_NUMBER() OVER (PARTITION BY EnvironmentId, MachineName  ORDER BY CreatedDate) AS Sequence_Id
		                FROM Trans_System_Health
                        WHERE CONVERT(DATE, CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
                              --CreatedDate BETWEEN DATEADD(hh,{0},DATEADD(dd,DATEDIFF(dd,0,GETDATE()-1),0)) AND GETDATE()
                        ) tsh 
                WHERE MemoryViolation = 1 AND {1} AND EnvironmentId = {2} AND MachineName = '{3}')
                
                INSERT INTO @Result(id, EnvironmentId,MachineName, MemoryViolation, MemoryViolationStatus, DenseRankresult, CreatedDate)
                SELECT id, EnvironmentId,MachineName, MemoryViolation, MemoryViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
                UPDATE a SET 
	                MemoryViolationStatus =  {5}
	                OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName, INSERTED.Id
                FROM Trans_System_Health a
                WHERE a.Id IN 
                    (SELECT TOP({4}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
						WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {4})
							) d  ORDER by d.createdDate)
                "; // for 3 and 6 records

        public static string GetAndUpdateViolatedRecordsByStorage = @"
                DECLARE @Result TABLE
                (
	                Id uniqueidentifier,
	                EnvironmentId int,
	                MachineName nvarchar(255),
	                StorageViolation int,
	                StorageViolationStatus int,
	                DenseRankresult int,
                    CreatedDate DateTime
                )
                ;WITH CTE AS (
                SELECT tsh.Id, tsh.EnvironmentId, tsh.MachineName, tsh.StorageViolation, tsh.Sequence_Id, tsh.StorageViolationStatus, CreatedDate, 
	                tsh.Sequence_Id - DENSE_RANK() OVER(PARTITION BY tsh.EnvironmentId, tsh.MachineName ORDER BY tsh.Sequence_Id) as DenseRankresult
                FROM (SELECT Id, EnvironmentId, MachineName, StorageViolation, StorageViolationStatus, CreatedDate, 
			                ROW_NUMBER() OVER (PARTITION BY EnvironmentId, MachineName  ORDER BY CreatedDate) AS Sequence_Id
		                FROM Trans_System_Health
                        WHERE CONVERT(DATE, CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP)
                            --CreatedDate BETWEEN DATEADD(hh,{0},DATEADD(dd,DATEDIFF(dd,0,GETDATE()-1),0)) AND GETDATE()
                        ) tsh 
                WHERE StorageViolation = 1 AND {1} AND EnvironmentId = {2} AND MachineName = '{3}')
                
                INSERT INTO @Result(id, EnvironmentId,MachineName, StorageViolation, StorageViolationStatus, DenseRankresult, CreatedDate)
                SELECT id, EnvironmentId,MachineName, StorageViolation, StorageViolationStatus, DenseRankresult, CreatedDate FROM CTE 
	
                UPDATE a SET 
	                StorageViolationStatus =  {5}
	                OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName, INSERTED.Id
                FROM Trans_System_Health a
                WHERE a.Id IN 
                    (SELECT TOP({4}) d.id FROM (SELECT b.Id, createdDate FROM @Result b 
						WHERE b.DenseRankresult IN (SELECT c.DenseRankresult FROM @Result c GROUP BY c.DenseRankresult HAVING COUNT(*) >= {4})
							) d  ORDER by d.createdDate)
                "; // for 3 and 6 records

        public static string GetAndUpdateSystemError = @"
                UPDATE a SET 
	                SystemViolationStatus =  1
	                OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName
                FROM Trans_System_Health a
                WHERE a.Id IN 
                    (SELECT TOP({2}) b.Id FROM Trans_System_Health b 
                           WHERE b.[status] = 0 AND (b.SystemViolationStatus != 1 OR b.SystemViolationStatus IS NULL )
                                AND b.EnvironmentId = {0} AND b.MachineName = '{1}' AND b.[Error_Message] IS NOT null )
                ";

        public static string GetAndUpdateServiceError = @"
                DECLARE  @result table(EnvironmentId int,MachineName nvarchar(250), RequestID uniqueidentifier)
                DECLARE  @result2 table(EnvironmentId int,MachineName nvarchar(250),ProcessName nvarchar(max))
                UPDATE a SET
                    ServiceViolationStatus =  1
                    OUTPUT INSERTED.EnvironmentId, INSERTED.MachineName, INSERTED.RequestID into @result
                FROM Trans_System_Health a
                WHERE a.Id IN
                    (SELECT a.Id  FROM [Trans_System_Health] a
                           WHERE a.EnvironmentId = {0} AND a.MachineName = '{1}' 
                                AND a.ServiceViolation =  1
                                AND (a.ServiceViolationStatus != 1 OR a.ServiceViolationStatus IS NULL ))

                insert into @result2(EnvironmentId, MachineName,ProcessName)
				SELECT EnvironmentId, MachineName,ProcessName 
				FROM (SELECT a.EnvironmentId, a.MachineName,b.ProcessName
                FROM @result a
                JOIN [Trans_System_Health_Instance] b on a.RequestID=b.RequestID AND b.[Status] != 'Running'
                GROUP BY  a.EnvironmentId, a.MachineName,b.ProcessName)  c

				SELECT distinct EnvironmentId,MachineName,STUFF((
				 SELECT ',' + ProcessName
					FROM @result2
					FOR XML PATH('')
				 ), 1, 1, '') Service
				 FROM @result2 ";

        public static string SaveToMessageLog = @"
                INSERT INTO Trans_Message_Log (ID, CreatedDate, [Status], CreatedBy,  EmailTo, IsMailServer, 
						                EmailSubject, EmailBody, MachineName, EnvironmentId , SendType,Priority)
                VALUES(NEWID(), GetDate(), {0}, '{1}', '{2}', {3}, '{4}', N'{5}', '{6}', {7}, {8},{9} )
                "; // Insert to message log

        public static string GetDailyReportLogs = @"
                SELECT ID, [Status], CreatedBy, CreatedDate, EmailTo, EmailCC, EmailBCC, IsMailServer, EmailSubject,
                    EmailBody, Priority, MachineName, EnvironmentId, SendType
                FROM [Trans_Message_Log] WITH (NOLOCK)
                WHERE SendType = 1
                    AND CONVERT(DATE, CreatedDate) = (CASE  WHEN  datepart(HH,getdate()) = 23 THEN CONVERT(DATE, CURRENT_TIMESTAMP)  ELSE CONVERT(DATE, DATEADD(DAY,-1,CURRENT_TIMESTAMP)) END)
                    AND EnvironmentId = {0}
                    AND MachineName = '{1}'
                ORDER BY CreatedDate 
                ";  

        
       

        public static string GetStatisticsByCPU = @"
                SELECT se.Name As EnvironmentName, tsh.MachineName, AVG(tsh.CPUInfo) As Average
                FROM Trans_System_Health tsh WITH (NOLOCK)
                INNER JOIN Sys_Environment se WITH (NOLOCK) ON tsh.EnvironmentID = se.ID
               WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
                AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
                AND se.Id = {0} 
                AND tsh.MachineName = '{1}'
                GROUP BY se.Name, tsh.MachineName
                ";

        public static string GetStatisticsByMemory = @"
                SELECT se.Name As EnvironmentName, tsh.MachineName, AVG(tsh.MemoryInfo) As Average 
                FROM Trans_System_Health tsh WITH (NOLOCK)
                INNER JOIN Sys_Environment se WITH (NOLOCK) ON tsh.EnvironmentID = se.ID
              WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
                AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
                        AND se.Id = {0} 
                        AND tsh.MachineName = '{1}'
                GROUP BY se.Name, tsh.MachineName
                ";

        public static string GetStatisticsByStorage = @"
                SELECT se.Name  As EnvironmentName, tsh.MachineName, AVG(tsh.StorageInfo) As Average 
                FROM Trans_System_Health tsh WITH (NOLOCK)
                INNER JOIN Sys_Environment se WITH (NOLOCK) ON tsh.EnvironmentID = se.ID
               WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
                AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
                        AND se.Id = {0} 
                        AND tsh.MachineName = '{1}'
                GROUP BY se.Name, tsh.MachineName
                ";

        public static string GetStatisticsByErrorNumbers = @"
        SELECT se.Name  As EnvironmentName, '' MachineName,  AVG(tsh.ErrorNumbers) As Average 
        FROM Trans_Data_Summary  tsh WITH (NOLOCK)
        INNER JOIN Sys_Environment se WITH (NOLOCK) ON tsh.EnvironmentID = se.ID
        WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
        AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
        AND se.Id = {0} 
        AND tsh.ErrorNumbers > 0
         GROUP BY se.Name
        ";
        public static string GetStatisticsByIntergrationErrorNumbers = @"
        SELECT se.Name  As EnvironmentName, '' MachineName,  AVG(tsh.IntergrationErrorNumbers) As Average 
        FROM Trans_Data_Summary  tsh WITH (NOLOCK)
        INNER JOIN Sys_Environment se WITH (NOLOCK) ON tsh.EnvironmentID = se.ID
        WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
        AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
        AND se.Id = {0} 
        AND tsh.IntergrationErrorNumbers > 0
        GROUP BY se.Name
        ";
        public static string GetStatisticsByPendingTransactions = @"
        SELECT se.Name  As EnvironmentName, '' MachineName,  AVG(tsh.PendingTransactions) As Average 
        FROM Trans_Data_Summary  tsh WITH (NOLOCK)
        INNER JOIN Sys_Environment se WITH (NOLOCK) ON tsh.EnvironmentID = se.ID
        WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
        AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
        AND se.Id = {0} 
        AND tsh.PendingTransactions > 0
        GROUP BY se.Name
        ";


              

        public static string GetResendMessageLogs = @"
                SELECT ID, EmailTo, EmailSubject, EmailBody, MachineName, EnvironmentId
                FROM [Trans_Message_Log] WITH (NOLOCK)
                WHERE [Status] = 0
                AND CONVERT(DATE, CreatedDate) = CONVERT(DATE, CURRENT_TIMESTAMP);
                ";  // to resend -> better to use retry logic in code


        public static string GetStatistbyFreeDisk = @"
                SELECT se.Name AS EnvironmentName, tsh.MachineName, AVG(tshs.TotalFreeSpace * 100 / tshs.TotalSize) As Average,tshs.DriveName
                FROM Trans_System_Health tsh
                LEFT JOIN Trans_System_Health_Storage AS tshs  WITH (NOLOCK) ON tshs.RequestID = tsh.RequestID
                LEFT JOIN Sys_Environment AS se  WITH (NOLOCK) ON se.ID = tsh.EnvironmentID
              WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
                AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
                    and tsh.EnvironmentID = {0} 
                    AND tsh.MachineName = '{1}' 
                    AND tshs.DriveName IS NOT NULL
                GROUP BY se.Name,tshs.DriveName, tsh.MachineName
                "; 
        
        public static string GetStatistbyTransaction = @"
                 SELECT se.Name AS EnvironmentName, tsh.MachineName, AVG(tshs.TransactionElapsed) As Average 
                FROM Trans_System_Health tsh  WITH (NOLOCK)
                LEFT JOIN Trans_Data_Health  AS tshs  WITH (NOLOCK) ON tshs.RequestID = tsh.RequestID
                LEFT JOIN Sys_Environment AS se  WITH (NOLOCK) ON se.ID = tsh.EnvironmentID
                WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
                AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
                    and tsh.EnvironmentID = {0} 
                    AND tsh.MachineName ='{1}' 
                GROUP BY se.Name,tsh.MachineName
                "; 
        public static string GetStatistbyFileTransfer = @"
                 SELECT se.Name AS EnvironmentName, tsh.MachineName, AVG(tshs.TransferElapsed) As Average 
                FROM Trans_System_Health tsh
                LEFT JOIN Trans_Data_Health  AS tshs  WITH (NOLOCK) ON tshs.RequestID = tsh.RequestID
                LEFT JOIN Sys_Environment AS se  WITH (NOLOCK) ON se.ID = tsh.EnvironmentID
             WHERE CONVERT (DATE,tsh.CreatedDate) > CONVERT (DATE,dateadd(day,datediff(day,1,GETDATE()),0))
                AND CONVERT (DATE,tsh.CreatedDate) <= CONVERT (DATE,dateadd(day,datediff(day,0,GETDATE()),0))
                    and tsh.EnvironmentID = {0} 
                    AND tsh.MachineName ='{1}' 
                GROUP BY se.Name,tsh.MachineName
                ";

        public static string MoveDataTableHistory = @"
           DECLARE @TbQueryCreateColummExists AS TABLE ([Index] int IDENTITY(1,1) PRIMARY KEY,TABLE_NAME VARCHAR(128),COLUMN_NAME VARCHAR(128),DATA_TYPE VARCHAR(128),  QUERY NVARCHAR(MAX))

			DECLARE @tableCheck AS TABLE ( [Index] int IDENTITY(1,1) PRIMARY KEY, TABLE_NAME VARCHAR(125))
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Config Table backup - Start'
			INSERT INTO @tableCheck
			(
				TABLE_NAME
			)
			VALUES 
			('Trans_Data_Health'),
			('Trans_System_Health'),
			('Trans_Message_Log'),
			('Trans_System_Health_Instance'),
			('Trans_System_Health_Storage'),
            ('Trans_Data_Summary'),
            ('Trans_Data_Status'),
            ('Trans_Data_Integration')


			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Config Table backup - End'
 
			DECLARE @tableName VARCHAR(128)
			DECLARE @cnt INT = 0;
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Render query create column - Start'
			WHILE @cnt < (SELECT COUNT(1) FROM @tableCheck)
			BEGIN
	
				SELECT @tableName= t.TABLE_NAME FROM @tableCheck AS t WHERE t.[Index] = @cnt + 1
	
				INSERT INTO @TbQueryCreateColummExists (TABLE_NAME, COLUMN_NAME, DATA_TYPE, [QUERY])
				SELECT  'history.' +c.TABLE_NAME +'_Archive',c.COLUMN_NAME,
				c.DATA_TYPE + IIF(c.CHARACTER_MAXIMUM_LENGTH IS NULL,'',IIF(c.CHARACTER_MAXIMUM_LENGTH = -1,'(max)','('+CONVERT(VARCHAR(12),c.CHARACTER_MAXIMUM_LENGTH )+')') ),
				 '
				 ALTER TABLE history.' +@tableName+'_Archive ADD ['+COLUMN_NAME+'] '+c.DATA_TYPE + IIF(c.CHARACTER_MAXIMUM_LENGTH IS NULL,'',IIF(c.CHARACTER_MAXIMUM_LENGTH = -1,'(max)','('+CONVERT(VARCHAR(12),c.CHARACTER_MAXIMUM_LENGTH )+')') ) 'Query'
				FROM INFORMATION_SCHEMA.COLUMNS AS c
				OUTER APPLY (
					SELECT b.TABLE_NAME ,b.COLUMN_NAME COLUMN_NAME_NotExists
					FROM INFORMATION_SCHEMA.COLUMNS AS b 
					WHERE TABLE_NAME =  @tableName +'_Archive' AND c.COLUMN_NAME = b.COLUMN_NAME 
				) b
				WHERE c.TABLE_NAME = @tableName
				AND b.COLUMN_NAME_NotExists IS NULL
			   SET @cnt = @cnt + 1;
			END;
			SET @cnt = 0
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Render query create column - End'
 


			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query create column - Start'
			WHILE @cnt < (SELECT COUNT(1) FROM @TbQueryCreateColummExists)
			BEGIN
				DECLARE @query NVARCHAR(max)
				SELECT @query= t.[QUERY] FROM @TbQueryCreateColummExists AS t WHERE t.[Index] = @cnt + 1
	
				PRINT @query
				
	
				SET @cnt = @cnt +1
			end
			SET @cnt = 0
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query create column - End'


			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query move data to table bk - Start'
			WHILE @cnt < (SELECT COUNT(1) FROM @tableCheck)
			BEGIN
	 
				SELECT @tableName= t.TABLE_NAME FROM @tableCheck AS t WHERE t.[Index] = @cnt + 1
	
 
				DECLARE @query_column NVARCHAR(MAX)
				DECLARE @query_insert_data NVARCHAR(MAX)

				SELECT TOP 1 @query_column =  STUFF((
					SELECT ',[' + c.COLUMN_NAME+']'
					FROM INFORMATION_SCHEMA.COLUMNS AS c 
                    INNER JOIN  INFORMATION_SCHEMA.COLUMNS AS h ON h.TABLE_NAME = @tableName + '_Archive' AND h.COLUMN_NAME = c.COLUMN_NAME
                    WHERE c.TABLE_NAME = @tableName 
                    ORDER BY c.COLUMN_NAME
					FOR XML PATH('')
					), 1, 1, '')  
				FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName

				SET @query_insert_data = '
				INSERT INTO history.'+@tableName+'_Archive ('+@query_column+')
				SELECT '+@query_column+' 
				FROM '+@tableName+' as r
				WHERE  '+IIF(@tableName in ( 'Trans_System_Health_Storage','Trans_Data_Integration') ,'CreateDate','CreatedDate')	+' < DATEADD(DAY,-{0},CURRENT_TIMESTAMP)
	
				DELETE FROM '+@tableName+' 
				WHERE  '+IIF(@tableName in ( 'Trans_System_Health_Storage','Trans_Data_Integration') ,'CreateDate','CreatedDate')	+' < DATEADD(DAY,-{0},CURRENT_TIMESTAMP)
				'
			 
				PRINT @query_insert_data
				EXEC (@query_insert_data)
				SET @cnt = @cnt +1
			end
			PRINT CONVERT(VARCHAR(32),GETDATE(),108) + ' - Exec query move data to table bk - End'
        ";

    }
}
