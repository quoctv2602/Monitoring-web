using Notifications.DAL;
using Notifications.DAL.EFModel;
using Notifications.DAL.Enums;
using Notifications.EmailService;
using Notifications.Options;
using Quartz;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Monitoring_Notifications.Jobs
{
    //[DisallowConcurrentExecution]
    public class SummaryReportJob : IJob
    {
        private readonly ILogger _logger;
        private readonly Configs _configs;
        private readonly IEmailProvider _emailService;
        private readonly INotificationDataFacade _notificationDataFacade;
        public SummaryReportJob(ILogger<SummaryReportJob> logger, Configs configs,
                            IEmailProvider emailService,
                            INotificationDataFacade notificationDataFacade)
        {
            _logger = logger;
            _emailService = emailService;
            _notificationDataFacade = notificationDataFacade;
            _configs = configs;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Daily Report Job started! - " + DateTime.Now.ToString());
            try
            {
                ProcessDailyReport();
            }
            catch (Exception ex)
            {
                _logger.LogError("Daily Report Job! - " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            _logger.LogInformation("Daily Report Job ended! - " + DateTime.Now.ToString());
            return Task.CompletedTask;
        }
        private void ProcessDailyReport()
        {
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            List<SysNodeSetting> sysNodeSettings = _notificationDataFacade.GetNodeList();
            foreach (var sysNodeSetting in sysNodeSettings)
            {
                _logger.LogInformation("ProcessDailyReport EnvironmentId: " + sysNodeSetting.EnvironmentId + " MachineName: " + sysNodeSetting.MachineName);
                try
                {
                    bool isSendError = false;
                    List<TransMessageLog> messageLogs = _notificationDataFacade.GetDailyReportLogs(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);

                    // Send email
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(sysNodeSetting.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(sysNodeSetting.EnvironmentId);
                    SysNodeSetting node = _notificationDataFacade.GetNodeSettings(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);

                    SysNodeSetting nodeTransaction = _notificationDataFacade.GetNodeSettings(sysNodeSetting.EnvironmentId);

                    if(node.ReportAlias != nodeTransaction.ReportAlias)
                    {
                        node.ReportAlias = node.ReportAlias + ", " + nodeTransaction.ReportAlias;
                    }
                    string stringMail = node.ReportEmail + ";" + nodeTransaction.ReportEmail;
                    string[] ListMail = stringMail.Split(';');

                    List<string> uniqueList = ListMail.Distinct().ToList();
                    node.ReportEmail = string.Join(";", uniqueList);


                    SysMonitoring CPUMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.CPU);
                    SysMonitoring memoryMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.Memory);
                    SysMonitoring storageMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.Storage);

                    SysMonitoring freeDiskMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.FreeDisk);
                    SysMonitoring EDItoASCIIMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.Transaction);
                    SysMonitoring FileTransferMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.FileTransfer);


                    SysMonitoring ErrorNumbersMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.FailedTransaction);
                    SysMonitoring IntergrationErrorNumbersMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.IntergrationErrorTransaction);
                    SysMonitoring PendingTransactionsMon = _notificationDataFacade.GetMonitoring((int)MonitoringType.PendingTransaction);




                    StatisticsDTO CPUStatistics = _notificationDataFacade.GetStatisticsByCPU(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);
                    StatisticsDTO memoryStatistics = _notificationDataFacade.GetStatisticsByMemory(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);
                    StatisticsDTO storageStatistics = _notificationDataFacade.GetStatisticsByStorage(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);
                    StatisticsDTO EDItoASCIIMonStatistics = _notificationDataFacade.GetStatistbyTransaction(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);
                    StatisticsDTO FileTransferStatistics = _notificationDataFacade.GetStatistbyFileTransfer(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);


                    StatisticsDTO ErrorNumbersStatistics = _notificationDataFacade.GetStatisticsByErrorNumbers(sysNodeSetting.EnvironmentId);
                    StatisticsDTO IntergrationErrorNumbersStatistics = _notificationDataFacade.GetStatisticsByIntergrationErrorNumbers(sysNodeSetting.EnvironmentId);
                    StatisticsDTO PendingTransactionsStatistics = _notificationDataFacade.GetStatisticsByPendingTransactions(sysNodeSetting.EnvironmentId);


                    List<StatisticsDTO_FreeDisk> FreeDiskStatistics = _notificationDataFacade.GetStatisticsByFreeDisk(sysNodeSetting.EnvironmentId, sysNodeSetting.MachineName);


                    Tuple<string, string> formatResult = EmailHandler.FormatTemplate4Email(env.Name,
                                            sysNodeSetting.MachineName, messageLogs,
                                            CPUMon.Name, CPUStatistics.Average, CPUMon.Unit,
                                            memoryMon.Name, memoryStatistics.Average, memoryMon.Unit,
                                            storageMon.Name, storageStatistics.Average, storageMon.Unit,
                                            node.ReportAlias, NotificationMonitoringURL,
                                            freeDiskMon, FreeDiskStatistics,
                                            EDItoASCIIMon, EDItoASCIIMonStatistics,
                                            FileTransferMon, FileTransferStatistics,
                                            ErrorNumbersMon, ErrorNumbersStatistics,
                                            IntergrationErrorNumbersMon, IntergrationErrorNumbersStatistics,
                                            PendingTransactionsMon, PendingTransactionsStatistics

                                            );
                    try
                    {


                        EmailHandler.SendEmail(emailConfig, node.ReportEmail,
                                formatResult.Item1, formatResult.Item2, _emailService);
                    }
                    catch (Exception ex)
                    {
                        isSendError = true;
                        _logger.LogError("ProcessDailyReport method send mail: " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }
                    // Insert into messagelog
                    TransMessageLog messageLog = new TransMessageLog();
                    if (isSendError == true)
                        messageLog.Status = (int)MessageLogStatus.SentButError;
                    else
                        messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                    messageLog.CreatedBy = "Notification Service";
                    messageLog.EmailTo = node.ReportEmail;
                    messageLog.IsMailServer = true;
                    messageLog.EmailSubject = formatResult.Item1;
                    messageLog.EmailBody = formatResult.Item2;
                    messageLog.EnvironmentId = sysNodeSetting.EnvironmentId;
                    messageLog.MachineName = sysNodeSetting.MachineName;
                    messageLog.SendType = (int)SendType.DailyReport;
                    messageLog.Priority = (int)MessagePriority.Normal;
                    _notificationDataFacade.AddMessageLog(messageLog);
                }
                catch (Exception ex)
                {
                    _logger.LogError("ProcessDailyReport method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace );

                }
            }
        }
    }
}
