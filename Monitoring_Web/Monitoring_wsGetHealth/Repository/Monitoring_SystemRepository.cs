using Monitoring_Common.DataContext;
using Monitoring_wsGetHealth.App_Code;
using Monitoring_wsGetHealth.Model;
using static Monitoring_Common.Service.WindowCounters;

namespace Monitoring_wsGetHealth.Repository
{
    public class Monitoring_SystemRepository
    {


        public static string CreateRequestID(int EnvironmentID, string MachineName)
        {
            string q = @"  
                            DECLARE @RequestID UNIQUEIDENTIFIER = NEWID()
                            INSERT INTO Trans_System_Health (ID, RequestID, EnvironmentID, MachineName, [Status] )
                            VALUES (NEWID(), @RequestID, {0}, N'{1}', 0 )
                            SELECT @RequestID as RequestID
                    ";

            string query = string.Format(q, EnvironmentID, MachineName);

            return Convert.ToString(SqlHelper.ExecuteQuery(GlobalSettings.ConnectionStrings(), query).Tables[0].Rows[0][0]);
        }

        public static int ImportMonitoring_System_Fail(string RequestID, string Message)
        {
            string query = " UPDATE Trans_System_Health SET [Status] = 0  ,[Error_Message] = N'" + Message + "' WHERE RequestID = '" + RequestID + "'";

            return SqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionStrings(), query);
        }


        static string NullToString(object Value)
        {
            return Value == null ? "" : Value.ToString();
        }
        public static int ImportMonitoring_System(string RequestID, int EnvironmentID, string MachineName, string IPAddress, int CPUInfo, int MemoryInfo, string RequestTime, string ResponseTime, int StorageInfo, int NumberOfTransactionFail, string ContentData, int Status, string Message, List<CounterDetail>? detail, List<DiskModel>? disk, TransferModel? Transfer, TransactionEDItoASCIIModel? EDItoASCII)
        {
            var violationService = detail.FirstOrDefault(c => !c.Status.Equals("Running"));
            string Trans_System_Health = @"
                      DECLARE @CPU INT;DECLARE @RAM INT;DECLARE @DISK INT;DECLARE @FreeDisk INT; DECLARE @Transfer INT; DECLARE @EDItoASCII INT;
                      DECLARE @CPU_Condition int;DECLARE @RAM_Condition int;DECLARE @DISK_Condition int; DECLARE @FreeDisk_Condition int; DECLARE @Transfer_Condition int;DECLARE @EDItoASCII_Condition INT;

                      SELECT @CPU = mec.Threshold, @CPU_Condition =   Condition  FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) WHERE  mec.EnvironmentID = '" + EnvironmentID + @"' AND mec.MachineName = N'"+ MachineName + @"' AND mec.MonitoringType = 1
                      SELECT @RAM = mec.Threshold, @RAM_Condition =  Condition  FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) WHERE  mec.EnvironmentID = '" + EnvironmentID + @"' AND mec.MachineName = N'"+ MachineName + @"' AND mec.MonitoringType = 2
                      SELECT @DISK = mec.Threshold, @DISK_Condition =  Condition  FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) WHERE  mec.EnvironmentID ='" + EnvironmentID + @"' AND mec.MachineName = N'"+ MachineName + @"' AND mec.MonitoringType = 3

                        SELECT @FreeDisk = mec.Threshold, @FreeDisk_Condition =  Condition  FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) WHERE  mec.EnvironmentID ='" + EnvironmentID + @"' AND mec.MachineName = N'"+ MachineName + @"' AND mec.MonitoringType = 5

                        SELECT @Transfer = mec.Threshold, @Transfer_Condition =  Condition  FROM  Sys_Threshold_Rule AS mec   WITH (NOLOCK) WHERE  mec.EnvironmentID ='" + EnvironmentID + @"' AND mec.MachineName = N'"+ MachineName + @"' AND mec.MonitoringType = 6

                        SELECT @EDItoASCII = mec.Threshold, @EDItoASCII_Condition =  Condition  FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) WHERE  mec.EnvironmentID ='" + EnvironmentID + @"' AND mec.MachineName = N'"+ MachineName + @"' AND mec.MonitoringType = 4
     

                        UPDATE Trans_System_Health
                        SET 
                            IPAddress = N'" + IPAddress + @"',
                            CPUInfo = "+ CPUInfo + @",
                            CPUViolation = CASE  WHEN @CPU_Condition = 1 THEN (CASE WHEN " + CPUInfo + @" > @CPU THEN 1 ELSE 0 END) ELSE (CASE WHEN " + CPUInfo + @" < @CPU THEN 1 ELSE 0 END) END,
                            MemoryInfo = " + MemoryInfo + @",
                            MemoryViolation =   CASE  WHEN @RAM_Condition = 1 THEN (CASE WHEN " + MemoryInfo + @" > @RAM THEN 1 ELSE 0 END) ELSE (CASE WHEN " + MemoryInfo + @" < @RAM THEN 1 ELSE 0 END) END,
                            StorageInfo = " + StorageInfo + @",
                            StorageViolation =  CASE  WHEN @DISK_Condition = 1 THEN (CASE WHEN " + StorageInfo + @" > @DISK THEN 1 ELSE 0 END) ELSE (CASE WHEN " + StorageInfo + @" < @DISK THEN 1 ELSE 0 END) END,
                            RequestTime = '" + RequestTime + @"',
                            ResponseTime = '"+ ResponseTime + @"',
                            ContentData =  N'"+ ContentData.Replace("'","") + @"',
                            ServiceViolation = N'"+ (violationService!=null?1:0) + @"',
                            [Status] = " + Status + @",
                            [Error_Message] = "+(Message ==null ? "null" : "N'"+Message+"'") +@"
                        WHERE RequestID = '" + RequestID + @"'
              ";

            string Trans_System_Health_Instance = @"   
                        INSERT INTO Trans_System_Health_Instance   (ID,RequestID,EnvironmentID,MachineName,IPAddress,   ProcessName, CPU, Ram, Storage, Instance ,[Status],CreatedDate)
                        VALUES
                      ";

            for (int i = 0; i < detail.Count; i++)
            {
                //Trans_System_Health_Instance += "( NEWID(),'"+ RequestID + "',"+ EnvironmentID + ",N'"+ MachineName + "',N'"+ IPAddress + "',   N'"+ detail[i].ProcessName + "', "+ detail[i].CPUInfo + ", "+ detail[i].MemoryInfo + ", "+ detail[i].StorageInfo + ", "+ detail[i].CountInstance + ",'"+ detail[i].Status + "' ),";

                Trans_System_Health_Instance += "( NEWID(),'" + RequestID + "'," + EnvironmentID + ",N'" + MachineName + "',N'" + IPAddress + "',   N'" + detail[i].ProcessName + "', " + "null" + ", " + "null" + ", " + "null" + ", " + "null" + ",'" + detail[i].Status + "', GETDATE() ),";

            }
            Trans_System_Health_Instance = Trans_System_Health_Instance.Substring(0, Trans_System_Health_Instance.Length - 1);


            string Trans_System_Health_Storage = @"
                            INSERT INTO Trans_System_Health_Storage
                            (ID, RequestID, EnvironmentID, MachineName, IPAddress, DriveName, VolumeLabel, TotalSize, TotalFreeSpace,Violation, CreateDate )
                            VALUES
                        ";
            for (int i = 0; i < disk.Count; i++)
            {
                int Violation = Convert.ToInt32(Convert.ToDecimal( disk[i].TotalFreeSpace) / Convert.ToDecimal(disk[i].TotalSize) * 100);
                Trans_System_Health_Storage += "(NEWID(),'" + RequestID + "'," + EnvironmentID + ",N'" + MachineName + "',N'" + IPAddress + "','" + disk[i].DriveName + "','" + disk[i].VolumeLabel + "'," + disk[i].TotalSize + "," + disk[i].TotalFreeSpace + "," +
                    "CASE  WHEN @FreeDisk_Condition = 1 THEN (CASE WHEN " + Violation + @" > @FreeDisk THEN 1 ELSE 0 END) ELSE (CASE WHEN " + Violation + @" < @FreeDisk THEN 1 ELSE 0 END) END" +
                    ",getdate()),";
            }
            //EDItoASCII_Condition

            string Transferquery = @"
                                         DECLARE @CreateDate DATETIME
                                         SELECT @CreateDate = tsh.CreatedDate FROM Trans_System_Health AS tsh WHERE tsh.RequestID = '"+ RequestID+"'" +
                                         "" +

                                         @"INSERT INTO Trans_Data_Health
                                             ( ID,RequestID, EnvironmentID, MachineName,IPAddress, 
                                                BeginFileTransfer, EndFileTransfer,
 	                                            FileTransferViolation,
 	                                          
                                                 [TransferElapsed],
 	                                            [Status],
 	                                            [Error_Message],
 	                                            CreatedDate,BeginTransaction,EndTransaction,TransactionElapsed,TransactionViolation
                                             )
                                             VALUES
                                             (
 	                                            NEWID(),'" + RequestID + "'," + EnvironmentID + ",N'" + MachineName + "',N'" + IPAddress + "','"+Transfer.start.ToString("yyyy-MM-dd HH:mm:ss:fff") +"','"+Transfer.end.ToString("yyyy-MM-dd HH:mm:ss:fff") + "'," +
                                                "CASE  WHEN @Transfer_Condition = 1 THEN (CASE WHEN " + Transfer.miliseconds + @" > @Transfer THEN 1 ELSE 0 END) ELSE (CASE WHEN " + Transfer.miliseconds + @" < @Transfer THEN 1 ELSE 0 END) END" + "," +
                                               Transfer.miliseconds.ToString()+", "+ Status + ", "+ (Transfer.Error == null && EDItoASCII.Error == null ? "null" : "N'Transfer: " + NullToString(Transfer.Error).Replace("'", "") + " - EDItoASCII: " + NullToString(EDItoASCII.Error).Replace("'","")+ "'") + ", @CreateDate ," +
                                                "'"+EDItoASCII.start.ToString("yyyy-MM-dd HH:mm:ss:fff") + "', '"+EDItoASCII.end.ToString("yyyy-MM-dd HH:mm:ss:fff") + "', "+EDItoASCII.miliseconds + ", CASE  WHEN @EDItoASCII_Condition = 1 THEN (CASE WHEN " + EDItoASCII.miliseconds + @" > @EDItoASCII THEN 1 ELSE 0 END) ELSE (CASE WHEN " + EDItoASCII.miliseconds + @" < @EDItoASCII THEN 1 ELSE 0 END) END" + 
                                                " )";

            Trans_System_Health_Storage = Trans_System_Health_Storage.Substring(0, Trans_System_Health_Storage.Length - 1);
            string query = Trans_System_Health  + Trans_System_Health_Instance + Trans_System_Health_Storage + Transferquery;
            return SqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionStrings(), query);
        }


    }
}
