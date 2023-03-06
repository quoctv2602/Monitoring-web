using Monitoring_Common.DataContext;
using Monitoring_wsGetHealth.App_Code;
using Monitoring_wsGetHealth.Model;

namespace Monitoring_wsGetHealth.Repository
{
    public class  Monitoring_TransactionRepository
    {
        public static string CreateRequestID(string Process, string EnvironmentID)
        {
            string q = @"  
                           INSERT INTO {0} (RequestID,EnvironmentID)
                            OUTPUT Inserted.RequestID, INSERTED.EnvironmentID
                            VALUES(NEWID(),{1})
                    ";
            string query = ""; 
;           if (Process == "GetSummary")
            {
                query = string.Format(q, "Trans_Data_Summary", EnvironmentID);
            }
          

            return Convert.ToString(SqlHelper.ExecuteQuery(GlobalSettings.ConnectionStrings(), query).Tables[0].Rows[0][0]);
        }

        public static int UpdateDataSummary(string RequestTime,string ResponseTime, string CIPFolow, string StartDate, string EndDate, string ContentData, string ErrorNumbers, string IntergrationErrorNumbers, string PendingTransactions, string Status, string Error_Message, string RequestID,string EnvironmentID)
        {
            string q = @"

                        DECLARE @ErrorNumbers INT;DECLARE @IntergrationErrorNumbers INT;DECLARE @PendingTransactions INT
                        DECLARE @ErrorNumbers_Condition INT;DECLARE @IntergrationErrorNumbers_Condition INT;DECLARE @PendingTransactions_Condition INT
 
                        SELECT @ErrorNumbers = mec.Threshold, @ErrorNumbers_Condition =   Condition  
                        FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) 
                        WHERE  mec.EnvironmentID = {12}
                        AND mec.MonitoringType = 7

                        SELECT @IntergrationErrorNumbers = mec.Threshold, @IntergrationErrorNumbers_Condition =   Condition  
                        FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) 
                        WHERE  mec.EnvironmentID = {12}
                        AND mec.MonitoringType = 8

                        SELECT @PendingTransactions = mec.Threshold, @PendingTransactions_Condition =   Condition  
                        FROM  Sys_Threshold_Rule AS mec  WITH (NOLOCK) 
                        WHERE  mec.EnvironmentID = {12}
                        AND mec.MonitoringType = 9


                        UPDATE Trans_Data_Summary 
                         SET 
                          RequestTime = '{0}',
                          ResponseTime = '{1}',
                          CIPFolow = '{2}',
                          StartDate = '{3}',
                          EndDate = '{4}',
                          ContentData = N'{5}',
                          ErrorNumbers = {6},
                          IntergrationErrorNumbers ={7},
                          PendingTransactions= {8},
                          [Status] ={9},
                          [Error_Message] = " + (string.IsNullOrEmpty(Error_Message) ? "null" : "N'{10}'")+ @" ,
                          ErrorNumbersViolation = CASE  WHEN @ErrorNumbers_Condition = 1 THEN (CASE WHEN " + ErrorNumbers + @" > @ErrorNumbers THEN 1 ELSE 0 END) ELSE (CASE WHEN " + ErrorNumbers + @" < @ErrorNumbers THEN 1 ELSE 0 END) END,
                          IntergrationErrorNumbersViolation = CASE  WHEN @IntergrationErrorNumbers_Condition = 1 THEN (CASE WHEN " + IntergrationErrorNumbers + @" > @IntergrationErrorNumbers THEN 1 ELSE 0 END) ELSE (CASE WHEN " + IntergrationErrorNumbers + @" < @IntergrationErrorNumbers THEN 1 ELSE 0 END) END,
                          PendingTransactionsViolation = CASE  WHEN @PendingTransactions_Condition = 1 THEN (CASE WHEN " + PendingTransactions + @" > @PendingTransactions THEN 1 ELSE 0 END) ELSE (CASE WHEN " + PendingTransactions + @" < @PendingTransactions THEN 1 ELSE 0 END) END
                         WHERE RequestID = '{11}'
            ";
            string query = string.Format(q, RequestTime, ResponseTime, CIPFolow,Convert.ToDateTime(StartDate).ToString(), Convert.ToDateTime(EndDate).ToString() , ContentData, ErrorNumbers, IntergrationErrorNumbers, PendingTransactions, Status, Error_Message, RequestID, EnvironmentID);
            return SqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionStrings(), query);
        }
        public static int UpdateDataSummary_Fail(string RequestTime, string ResponseTime, string CIPFolow, string StartDate, string EndDate, string ContentData,string Status, string Error_Message, string RequestID)
        {
            string q = @"
                        UPDATE Trans_Data_Summary 
                         SET 
                          RequestTime = '{0}',
                          ResponseTime = '{1}',
                          CIPFolow = '{2}',
                          StartDate = '{3}',
                          EndDate = '{4}',
                          ContentData = N'{5}',
                          [Status] ={6},
                          [Error_Message] = " + (string.IsNullOrEmpty(Error_Message) ? "null" : "N'{7}'") + @" 
                         WHERE RequestID = '{8}'
            ";
            string query = string.Format(q, RequestTime, ResponseTime, CIPFolow, Convert.ToDateTime(StartDate).ToString(), Convert.ToDateTime(EndDate).ToString(), ContentData,Status, Error_Message, RequestID);
            return SqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionStrings(), query);
        }


        public static int ImportDataListError(string RequestTime, string ResponseTime, string CIPFlow, string StartDate, string EndDate, string ContentData, string Status, string Error_Message, string RequestID, TransactionBaseListError data, string EnvironmentID)
        {
            string q = "";
            string query = "";
            if (Status == "1")
            {
                if (data.listTransactionError.Count > 0)
                {
                    q = @"
                        INSERT INTO Trans_Data_Status
                        (
	                        RequestID,RequestTime,ResponseTime,CIPFolow,StartDate,EndDate,
	                        ContentData,TransactionKey,ErrorStatus,ErrorTime,[Status],[Error_Message],EnvironmentID
	
                        )
                        VALUES
                        {0}
                  ";
                    string listDetail = "";
                    for (int i = 0; i < data.listTransactionError.Count; i++)
                    {
                        listDetail += string.Format(@"(
	                        '{0}','{1}','{2}','{3}','{4}','{5}',
	                        N'{6}','{7}',{8},'{9}',{10},null, {11}
                        ),", RequestID, RequestTime, ResponseTime, CIPFlow, Convert.ToDateTime(StartDate).ToString(), Convert.ToDateTime(EndDate).ToString(),
                            ContentData, data.listTransactionError[i].Guid, data.listTransactionError[i].ErrorStatus, data.listTransactionError[i].CreatedDate, 1, EnvironmentID);
                    }

                    query = string.Format(q, listDetail.Substring(0, listDetail.Length - 1));
                }
                else
                {
                    q = @"
                    INSERT INTO Trans_Data_Status
                    (
	                    RequestID,RequestTime,ResponseTime,CIPFolow,StartDate,EndDate,
	                    ContentData,[Status],[Error_Message],EnvironmentID
	
                    )
                    VALUES
                    (
	                    '{0}','{1}','{2}','{3}','{4}','{5}',
	                    N'{6}',{7},N'{8}',{9}
                    )
                  ";
                    query = string.Format(q, RequestID, RequestTime, ResponseTime, CIPFlow, Convert.ToDateTime(StartDate).ToString(), Convert.ToDateTime(EndDate).ToString(),
                            ContentData, 1, null, EnvironmentID);
                }
                
            }
            else
            {
                q = @"
                    INSERT INTO Trans_Data_Status
                    (
	                    RequestID,RequestTime,ResponseTime,CIPFolow,StartDate,EndDate,
	                    ContentData,[Status],[Error_Message],EnvironmentID
	
                    )
                    VALUES
                    (
	                    '{0}','{1}','{2}','{3}','{4}','{5}',
	                    N'{6}',{7},N'{8}',{9}
                    )
                  ";
                query = string.Format(q, RequestID,RequestTime, ResponseTime, CIPFlow, Convert.ToDateTime(StartDate).ToString(), Convert.ToDateTime(EndDate).ToString(),
                        ContentData,0,Error_Message, EnvironmentID);
            }
           
            return SqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionStrings(), query);
        }
    }
}
