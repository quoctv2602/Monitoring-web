using Notifications.DAL.EFModel;
using Notifications.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.DAL
{
    public interface INotificationDataFacade
    {
        EmailConfig GetEmailServer(int environmentId);
        SysNodeSetting GetNodeSettings(int environmentId, string machineName);
        SysNodeSetting GetNodeSettings(int environmentId);
        SysNodeSetting GetNodeSettings(int environmentId,int NodeType);
        List<SysNodeSetting> GetNodeList();
        SysEnvironment GetEnvironment(int environmentId);
        SysMonitoring GetMonitoring(int id);
        List<SysNotificationDetail> GetListMail(int KPIid);
        List<SysThresholdRule> GetThresholdRules(int monitoringType);
        TransSystemHealthDTO GetViolatedRecordsByCPU(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords);


          TransSystemHealthDTO GetViolatedRecordsByFreeDisk(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords);

              TransSystemHealthDTO GetViolatedRecordsByTransaction(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords);

            TransSystemHealthDTO GetViolatedRecordsByFileTransfer(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords);



        TransSystemHealthDTO GetViolatedRecordsByMemory(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords);
        TransSystemHealthDTO GetViolatedRecordsByStorage(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords);
        TransSystemHealthDTO GetSystemError(int environmentId, string machineName, int maxRecords);
        TransSystemHealthProcessService GetServiceError(int environmentId, string machineName, int maxRecords);
        List<TransMessageLog> GetDailyReportLogs(int environmentId, string machineName);
        StatisticsDTO GetStatisticsByCPU(int environmentId, string machineName);
        List<StatisticsDTO_FreeDisk> GetStatisticsByFreeDisk(int environmentId, string machineName);
        StatisticsDTO GetStatistbyTransaction(int environmentId, string machineName);
        StatisticsDTO GetStatistbyFileTransfer(int environmentId, string machineName);
        StatisticsDTO GetStatisticsByMemory(int environmentId, string machineName);
        StatisticsDTO GetStatisticsByStorage(int environmentId, string machineName);


    

        StatisticsDTO GetStatisticsByErrorNumbers(int environmentId);
        StatisticsDTO GetStatisticsByIntergrationErrorNumbers(int environmentId);
        StatisticsDTO GetStatisticsByPendingTransactions(int environmentId);




        TransSystemHealthDTO GetViolatedRecordsByErrorNumbers(
                      string statusBeforeStr, int environmentId, 
                          int? thresholdCount, int statusAfter, int maxRecords);
        TransSystemHealthDTO GetViolatedRecordsByIntergrationErrorNumbers(
                      string statusBeforeStr, int environmentId, 
                          int? thresholdCount, int statusAfter, int maxRecords);
        TransSystemHealthDTO GetViolatedRecordsByPendingTransactions(
                  string statusBeforeStr, int environmentId,
                      int? thresholdCount, int statusAfter, int maxRecords);

   
        void AddMessageLog(TransMessageLog message);
    }
}
