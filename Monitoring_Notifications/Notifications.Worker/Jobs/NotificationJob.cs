using Notifications.DAL;
using Notifications.DAL.EFModel;
using Notifications.DAL.Enums;
using Notifications.EmailService;
using Notifications.Options;
using Org.BouncyCastle.Security;
using Quartz;
using System.Xml.Linq;
using static Org.BouncyCastle.Math.EC.ECCurve;


namespace Monitoring_Notifications.Jobs
{
    [DisallowConcurrentExecution]
    public class NotificationJob : IJob
    {
        private readonly ILogger _logger;
        private readonly Configs _configs;
        private readonly IEmailProvider _emailService;
        private readonly INotificationDataFacade _notificationDataFacade;
        public NotificationJob(ILogger<NotificationJob> logger, Configs configs,
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
            _logger.LogInformation("Notification Job started! - " + DateTime.Now.ToString());
            try
            {
                ProcessCPUViolations();
                ProcessMemoryViolation();
                ProcessStorageViolation();

                ProcessServiceViolation();

                ProcessFreeDiskViolation();
                ProcessTransactionViolation();
                ProcessFileTransferViolation();


                // sprint 3
                ProcessErrorNumbersViolation(); 
                ProcessIntergrationErrorNumbersViolation(); 
                ProcessPendingTransactionsViolation();
            }
            catch (Exception ex)
            {
                _logger.LogError("Notification Job! - " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            _logger.LogInformation("Notification Job ended! - " + DateTime.Now.ToString());
            return Task.CompletedTask;
        }
        private void ProcessErrorNumbersViolation() {
            string statusBeforeStr = " ( ErrorNumbersViolationStatus = 0 OR ErrorNumbersViolationStatus IS NULL ) ";
            int statusAfter = 1;  
         
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.FailedTransaction);
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByErrorNumbers(   statusBeforeStr, rule.EnvironmentId,
                                                                                                  rule.ThresholdCounter, statusAfter, maxRetryDBCount);
                if (result.EnvironmentId > 0) {

                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                    SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, 1);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.FailedTransaction);

                    Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                   "", rule.Threshold, rule.ThresholdCounter,
                                   mon.Name, mon.Unit, node.NotificationAlias, NotificationMonitoringURL);

                    try
                    {

                        EmailHandler.SendEmail(emailConfig, node.NotificationEmail,
                                formatResult.Item1, formatResult.Item2, _emailService);
                    }
                    catch (Exception ex)
                    {
                        isSendError = true;
                        _logger.LogError("ProcessErrorNumbersViolation method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }
                    // Insert into messagelog
                    TransMessageLog messageLog = new TransMessageLog();
                    if (isSendError == true)
                        messageLog.Status = (int)MessageLogStatus.SentButError;
                    else
                        messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                    messageLog.CreatedBy = "Notification Service";
                    messageLog.EmailTo = node.NotificationEmail;
                    messageLog.IsMailServer = true;
                    messageLog.EmailSubject = formatResult.Item1;
                    messageLog.EmailBody = formatResult.Item2;
                    messageLog.EnvironmentId = result.EnvironmentId;
                    messageLog.MachineName = result.MachineName;
                    messageLog.SendType = (int)SendType.Notification;
                    messageLog.Priority = (int)MessagePriority.Normal;
                    _notificationDataFacade.AddMessageLog(messageLog);
                }

            }



        }
        private void ProcessIntergrationErrorNumbersViolation() {
            string statusBeforeStr = " ( IntergrationErrorNumbersViolationStatus = 0 OR IntergrationErrorNumbersViolationStatus IS NULL ) ";
            int statusAfter = 1;
         
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.IntergrationErrorTransaction);
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByIntergrationErrorNumbers(statusBeforeStr, rule.EnvironmentId,
                                                                                                  rule.ThresholdCounter, statusAfter, maxRetryDBCount);
                if (result.EnvironmentId  > 0)
                {

                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                    SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, 1);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.IntergrationErrorTransaction);

                    Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                   "", rule.Threshold, rule.ThresholdCounter,
                                   mon.Name, mon.Unit, node.NotificationAlias, NotificationMonitoringURL);

                    try
                    {

                        EmailHandler.SendEmail(emailConfig, node.NotificationEmail,
                                formatResult.Item1, formatResult.Item2, _emailService);
                    }
                    catch (Exception ex)
                    {
                        isSendError = true;
                        _logger.LogError("ProcessIntergrationErrorNumbersViolation method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }
                    // Insert into messagelog
                    TransMessageLog messageLog = new TransMessageLog();
                    if (isSendError == true)
                        messageLog.Status = (int)MessageLogStatus.SentButError;
                    else
                        messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                    messageLog.CreatedBy = "Notification Service";
                    messageLog.EmailTo = node.NotificationEmail;
                    messageLog.IsMailServer = true;
                    messageLog.EmailSubject = formatResult.Item1;
                    messageLog.EmailBody = formatResult.Item2;
                    messageLog.EnvironmentId = result.EnvironmentId;
                    messageLog.MachineName = result.MachineName;
                    messageLog.SendType = (int)SendType.Notification;
                    messageLog.Priority = (int)MessagePriority.Normal;
                    _notificationDataFacade.AddMessageLog(messageLog);
                }

            }
        }
        private void ProcessPendingTransactionsViolation()
        {
            string statusBeforeStr = " ( PendingTransactionsViolationStatus = 0 OR PendingTransactionsViolationStatus IS NULL ) ";
            int statusAfter = 1;
           
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.PendingTransaction);
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByPendingTransactions(statusBeforeStr, rule.EnvironmentId,
                                                                                                  rule.ThresholdCounter, statusAfter, maxRetryDBCount);
                if (result.EnvironmentId > 0)
                {

                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                    SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, 1);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.PendingTransaction);

                    Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                   "", rule.Threshold, rule.ThresholdCounter,
                                   mon.Name, mon.Unit, node.NotificationAlias, NotificationMonitoringURL);

                    try
                    {

                        EmailHandler.SendEmail(emailConfig, node.NotificationEmail,
                                formatResult.Item1, formatResult.Item2, _emailService);
                    }
                    catch (Exception ex)
                    {
                        isSendError = true;
                        _logger.LogError("ProcessPendingTransactionsViolation method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }
                    // Insert into messagelog
                    TransMessageLog messageLog = new TransMessageLog();
                    if (isSendError == true)
                        messageLog.Status = (int)MessageLogStatus.SentButError;
                    else
                        messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                    messageLog.CreatedBy = "Notification Service";
                    messageLog.EmailTo = node.NotificationEmail;
                    messageLog.IsMailServer = true;
                    messageLog.EmailSubject = formatResult.Item1;
                    messageLog.EmailBody = formatResult.Item2;
                    messageLog.EnvironmentId = result.EnvironmentId;
                    messageLog.MachineName = result.MachineName;
                    messageLog.SendType = (int)SendType.Notification;
                    messageLog.Priority = (int)MessagePriority.Normal;
                    _notificationDataFacade.AddMessageLog(messageLog);
                }

            }

        }
        /// <summary>
        /// 
        /// </summary>
        private void ProcessCPUViolations()
        {
            string statusBeforeStr = " ( CPUViolationStatus = 0 OR CPUViolationStatus IS NULL ) "; // 0 or != 2 for 3 or 6 times
            int statusAfter = 1; // 1 or == 2 for 3 or 6 times
            int hours = _configs.AppSettings.HoursOfYesterday;
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.CPU);
            // Check monitoring type
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByCPU(
                            hours, statusBeforeStr, rule.EnvironmentId, rule.MachineName,
                            rule.ThresholdCounter, statusAfter, maxRetryDBCount);

                if (result.MachineName != null)
                {
                    // Send email
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                  //  SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.CPU);

                    List<SysNotificationDetail> listMail = _notificationDataFacade.GetListMail((int)MonitoringType.CPU);
                    try
                    {
                        foreach (var item in listMail)
                        {
                            Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                    result.MachineName, rule.Threshold, rule.ThresholdCounter,
                                    mon.Name, mon.Unit, item.NotificationAlias, NotificationMonitoringURL);
                            try
                            {

                                EmailHandler.SendEmail(emailConfig, item.Emails,
                                        formatResult.Item1, formatResult.Item2, _emailService);
                            }
                            catch (Exception ex)
                            {
                                isSendError = true;
                                _logger.LogError("ProcessCPUViolations method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                            }
                            // Insert into messagelog
                            TransMessageLog messageLog = new TransMessageLog();
                            if (isSendError == true)
                                messageLog.Status = (int)MessageLogStatus.SentButError;
                            else
                                messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                            messageLog.CreatedBy = "Notification Service";
                            messageLog.EmailTo = item.Emails;
                            messageLog.IsMailServer = true;
                            messageLog.EmailSubject = formatResult.Item1;
                            messageLog.EmailBody = formatResult.Item2;
                            messageLog.EnvironmentId = result.EnvironmentId;
                            messageLog.MachineName = result.MachineName;
                            messageLog.SendType = (int)SendType.Notification;
                            messageLog.Priority = (int)MessagePriority.Normal;
                            _notificationDataFacade.AddMessageLog(messageLog);
                        }
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("ProcessCPUViolations For mail method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }
                    
                }
            }
        }
        private void ProcessMemoryViolation()
        {
            string statusBeforeStr = " ( MemoryViolationStatus = 0 OR MemoryViolationStatus IS NULL ) "; // 0 or != 2 for 3 or 6 times
            int statusAfter = 1; // 1 or == 2 for 3 or 6 times
            int hours = _configs.AppSettings.HoursOfYesterday;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;

            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.Memory);
            // Check monitoring type
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByMemory(
                            hours, statusBeforeStr, rule.EnvironmentId, rule.MachineName,
                            rule.ThresholdCounter, statusAfter, maxRetryDBCount);

                if (result.MachineName != null)
                {
                    // Send email
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                    //SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.Memory);

                    List<SysNotificationDetail> listMail = _notificationDataFacade.GetListMail((int)MonitoringType.Memory);
                    try
                    {
                        foreach (var item in listMail)
                        {
                            Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                       result.MachineName, rule.Threshold, rule.ThresholdCounter,
                                       mon.Name, mon.Unit, item.NotificationAlias, NotificationMonitoringURL);
                            try
                            {
                                EmailHandler.SendEmail(emailConfig, item.Emails,
                                    formatResult.Item1, formatResult.Item2, _emailService);
                            }
                            catch (Exception ex)
                            {
                                isSendError = true;
                                _logger.LogError("ProcessMemoryViolation method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                            }
                            // Insert into messagelog
                            TransMessageLog messageLog = new TransMessageLog();
                            if (isSendError == true)
                                messageLog.Status = (int)MessageLogStatus.SentButError;
                            else
                                messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                            messageLog.CreatedBy = "Notification Service";
                            messageLog.EmailTo = item.Emails;
                            messageLog.IsMailServer = true;
                            messageLog.EmailSubject = formatResult.Item1;
                            messageLog.EmailBody = formatResult.Item2;
                            messageLog.EnvironmentId = result.EnvironmentId;
                            messageLog.MachineName = result.MachineName;
                            messageLog.SendType = (int)SendType.Notification;
                            messageLog.Priority = (int)MessagePriority.Normal;
                            _notificationDataFacade.AddMessageLog(messageLog);
                        }
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("ProcessMemoryViolation For mail method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }

                   
                }
            }
        }
        private void ProcessStorageViolation()
        {
            string statusBeforeStr = " ( StorageViolationStatus = 0 OR StorageViolationStatus IS NULL ) "; // 0 or != 2 for 3 or 6 times
            int statusAfter = 1; // 1 or == 2 for 3 or 6 times
            int hours = _configs.AppSettings.HoursOfYesterday;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;

            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.Storage);
            // Check monitoring type
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByStorage(
                            hours, statusBeforeStr, rule.EnvironmentId, rule.MachineName,
                            rule.ThresholdCounter, statusAfter, maxRetryDBCount);

                if (result.MachineName != null)
                {
                    // Send email
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                   // SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.Storage);

                    List<SysNotificationDetail> listMail = _notificationDataFacade.GetListMail((int)MonitoringType.Storage);
                    try
                    {
                        foreach (var item in listMail)
                        {
                            Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                      result.MachineName, rule.Threshold, rule.ThresholdCounter,
                                      mon.Name, mon.Unit, item.NotificationAlias, NotificationMonitoringURL);
                            try
                            {
                                EmailHandler.SendEmail(emailConfig, item.Emails,
                                    formatResult.Item1, formatResult.Item2, _emailService);
                            }
                            catch (Exception ex)
                            {
                                isSendError = true;
                                _logger.LogError("ProcessStorageViolation : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                            }

                            // Insert into messagelog
                            TransMessageLog messageLog = new TransMessageLog();
                            if (isSendError == true)
                                messageLog.Status = (int)MessageLogStatus.SentButError;
                            else
                                messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                            messageLog.CreatedBy = "Notification Service";
                            messageLog.EmailTo = item.Emails;
                            messageLog.IsMailServer = true;
                            messageLog.EmailSubject = formatResult.Item1;
                            messageLog.EmailBody = formatResult.Item2;
                            messageLog.EnvironmentId = result.EnvironmentId;
                            messageLog.MachineName = result.MachineName;
                            messageLog.SendType = (int)SendType.Notification;
                            messageLog.Priority = (int)MessagePriority.Normal;
                            _notificationDataFacade.AddMessageLog(messageLog);
                        }
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("ProcessStorageViolation For mail : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }

                  
                }
            }
        }
        private void ProcessSystemViolation()
        {
            List<SysNodeSetting> rules = _notificationDataFacade.GetNodeList();
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetSystemError(rule.EnvironmentId, rule.MachineName, maxRetryDBCount);
                if (result.MachineName != null)
                {
                    // Send email
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                    SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);

                    Tuple<string, string> formatResult = EmailHandler.FormatTemplate5Email(env.Name, result.MachineName, node.NotificationAlias, NotificationMonitoringURL);
                    try
                    {
                        EmailHandler.SendEmail(emailConfig, node.NotificationEmail,
                                formatResult.Item1, formatResult.Item2, _emailService);
                    }
                    catch (Exception ex)
                    {
                        isSendError = true;
                        _logger.LogError("ProcessSystemViolation method : " + ex.Message + " - Stacktrace: " + ex.StackTrace);
                    }
                    // Insert into messagelog
                    TransMessageLog messageLog = new TransMessageLog();
                    if (isSendError == true)
                        messageLog.Status = (int)MessageLogStatus.SentButError;
                    else
                        messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                    messageLog.CreatedBy = "Notification Service";
                    messageLog.EmailTo = node.NotificationEmail;
                    messageLog.IsMailServer = true;
                    messageLog.EmailSubject = formatResult.Item1;
                    messageLog.EmailBody = formatResult.Item2;
                    messageLog.EnvironmentId = result.EnvironmentId;
                    messageLog.MachineName = result.MachineName;
                    messageLog.SendType = (int)SendType.Notification;
                    messageLog.Priority = (int)MessagePriority.Urgent;
                    _notificationDataFacade.AddMessageLog(messageLog);
                }
            }
        }

        private void ProcessServiceViolation()
        {
            List<SysNodeSetting> rules = _notificationDataFacade.GetNodeList();
            string notificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthProcessService result = _notificationDataFacade.GetServiceError(rule.EnvironmentId, rule.MachineName, maxRetryDBCount);
                if (result.MachineName != null)
                {
                    // Send email
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                    SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);

                 

                    Tuple<string, string> formatResult = EmailHandler.FormatTemplate3Email(env.Name, result.MachineName, node.NotificationAlias, notificationMonitoringURL, result.Service);
                    try
                    {
                        EmailHandler.SendEmail(emailConfig, node.NotificationEmail,
                                formatResult.Item1, formatResult.Item2, _emailService);
                    }
                    catch (Exception ex)
                    {
                        isSendError = true;
                        _logger.LogError("ProcessServiceViolation method : " + ex.Message + " - Stacktrace: " + ex.StackTrace);
                    }
                    // Insert into messagelog
                    TransMessageLog messageLog = new TransMessageLog();
                    if (isSendError == true)
                        messageLog.Status = (int)MessageLogStatus.SentButError;
                    else
                        messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                    messageLog.CreatedBy = "Notification Service";
                    messageLog.EmailTo = node.NotificationEmail;
                    messageLog.IsMailServer = true;
                    messageLog.EmailSubject = formatResult.Item1;
                    messageLog.EmailBody = formatResult.Item2;
                    messageLog.EnvironmentId = result.EnvironmentId;
                    messageLog.MachineName = result.MachineName;
                    messageLog.SendType = (int)SendType.Notification;
                    messageLog.Priority = (int)MessagePriority.Urgent;
                    _notificationDataFacade.AddMessageLog(messageLog);
                }
            }
        }
        private void ProcessFreeDiskViolation()
        {
            string statusBeforeStr = " ( FreeDiskViolationStatus = 0 OR FreeDiskViolationStatus IS NULL ) "; // 0 or != 2 for 3 or 6 times
            int statusAfter = 1; // 1 or == 2 for 3 or 6 times
            int hours = _configs.AppSettings.HoursOfYesterday;
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.FreeDisk);
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByFreeDisk(
                            hours, statusBeforeStr, rule.EnvironmentId, rule.MachineName,
                            rule.ThresholdCounter, statusAfter, maxRetryDBCount);
                if (result.MachineName != null)
                {
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                   // SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.FreeDisk);

                    List<SysNotificationDetail> listMail = _notificationDataFacade.GetListMail((int)MonitoringType.FreeDisk);
                    try
                    {
                        foreach (var item in listMail)
                        {
                            Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                        result.MachineName, rule.Threshold, rule.ThresholdCounter,
                                        mon.Name, mon.Unit, item.NotificationAlias, NotificationMonitoringURL);
                            try
                            {

                                EmailHandler.SendEmail(emailConfig, item.Emails,
                                        formatResult.Item1, formatResult.Item2, _emailService);
                            }
                            catch (Exception ex)
                            {
                                isSendError = true;
                                _logger.LogError("ProcessFreeDiskViolations method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                            }
                            // Insert into messagelog
                            TransMessageLog messageLog = new TransMessageLog();
                            if (isSendError == true)
                                messageLog.Status = (int)MessageLogStatus.SentButError;
                            else
                                messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                            messageLog.CreatedBy = "Notification Service";
                            messageLog.EmailTo = item.Emails;
                            messageLog.IsMailServer = true;
                            messageLog.EmailSubject = formatResult.Item1;
                            messageLog.EmailBody = formatResult.Item2;
                            messageLog.EnvironmentId = result.EnvironmentId;
                            messageLog.MachineName = result.MachineName;
                            messageLog.SendType = (int)SendType.Notification;
                            messageLog.Priority = (int)MessagePriority.Normal;
                            _notificationDataFacade.AddMessageLog(messageLog);
                        }

                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("ProcessFreeDiskViolations For mail method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }
                }
            }
        }
        private void ProcessTransactionViolation()
        {
            string statusBeforeStr = " ( TransactionViolationStatus = 0 OR TransactionViolationStatus IS NULL ) "; // 0 or != 2 for 3 or 6 times
            int statusAfter = 1; // 1 or == 2 for 3 or 6 times
            int hours = _configs.AppSettings.HoursOfYesterday;
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.Transaction);
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByTransaction(
                            hours, statusBeforeStr, rule.EnvironmentId, rule.MachineName,
                            rule.ThresholdCounter, statusAfter, maxRetryDBCount);
                if (result.MachineName != null)
                {
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                   // SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.Transaction);

                    List<SysNotificationDetail> listMail = _notificationDataFacade.GetListMail((int)MonitoringType.Transaction);
                    try
                    {
                        foreach (var item in listMail)
                        {
                            Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                   result.MachineName, rule.Threshold, rule.ThresholdCounter,
                                   mon.Name, mon.Unit, item.NotificationAlias, NotificationMonitoringURL);
                            try
                            {

                                EmailHandler.SendEmail(emailConfig, item.Emails,
                                        formatResult.Item1, formatResult.Item2, _emailService);
                            }
                            catch (Exception ex)
                            {
                                isSendError = true;
                                _logger.LogError("ProcessTransactionViolations method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                            }
                            // Insert into messagelog
                            TransMessageLog messageLog = new TransMessageLog();
                            if (isSendError == true)
                                messageLog.Status = (int)MessageLogStatus.SentButError;
                            else
                                messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                            messageLog.CreatedBy = "Notification Service";
                            messageLog.EmailTo = item.Emails;
                            messageLog.IsMailServer = true;
                            messageLog.EmailSubject = formatResult.Item1;
                            messageLog.EmailBody = formatResult.Item2;
                            messageLog.EnvironmentId = result.EnvironmentId;
                            messageLog.MachineName = result.MachineName;
                            messageLog.SendType = (int)SendType.Notification;
                            messageLog.Priority = (int)MessagePriority.Normal;
                            _notificationDataFacade.AddMessageLog(messageLog);
                        }
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("ProcessTransactionViolations For mail method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }

                   
                }
            }
        }

        private void ProcessFileTransferViolation()
        {
            string statusBeforeStr = " ( FileTransferViolationStatus = 0 OR FileTransferViolationStatus IS NULL ) "; // 0 or != 2 for 3 or 6 times
            int statusAfter = 1; // 1 or == 2 for 3 or 6 times
            int hours = _configs.AppSettings.HoursOfYesterday;
            string NotificationMonitoringURL = _configs.AppSettings.NotificationMonitoringURL;
            int maxRetryDBCount = _configs.AppSettings.MaxRetryDBCount;
            List<SysThresholdRule> rules = _notificationDataFacade.GetThresholdRules((int)MonitoringType.FileTransfer);
            foreach (var rule in rules)
            {
                bool isSendError = false;
                TransSystemHealthDTO result = _notificationDataFacade.GetViolatedRecordsByFileTransfer(
                            hours, statusBeforeStr, rule.EnvironmentId, rule.MachineName,
                            rule.ThresholdCounter, statusAfter, maxRetryDBCount);
                if (result.MachineName != null)
                {
                    EmailConfig emailConfig = _notificationDataFacade.GetEmailServer(result.EnvironmentId);
                    SysEnvironment env = _notificationDataFacade.GetEnvironment(result.EnvironmentId);
                   // SysNodeSetting node = _notificationDataFacade.GetNodeSettings(result.EnvironmentId, result.MachineName);
                    SysMonitoring mon = _notificationDataFacade.GetMonitoring((int)MonitoringType.FileTransfer);

                    List<SysNotificationDetail> listMail = _notificationDataFacade.GetListMail((int)MonitoringType.FileTransfer);
                    try
                    {
                        foreach (var item in listMail)
                        {
                            Tuple<string, string> formatResult = EmailHandler.FormatTemplate1Email(env.Name,
                                  result.MachineName, rule.Threshold, rule.ThresholdCounter,
                                  mon.Name, mon.Unit, item.NotificationAlias, NotificationMonitoringURL);
                            try
                            {
                                
                                EmailHandler.SendEmail(emailConfig, item.Emails,
                                        formatResult.Item1, formatResult.Item2, _emailService);
                            }
                            catch (Exception ex)
                            {
                                isSendError = true;
                                _logger.LogError("ProcessFileTransferViolation method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                            }
                            // Insert into messagelog
                            TransMessageLog messageLog = new TransMessageLog();
                            if (isSendError == true)
                                messageLog.Status = (int)MessageLogStatus.SentButError;
                            else
                                messageLog.Status = (int)MessageLogStatus.SuccesfulySent;
                            messageLog.CreatedBy = "Notification Service";
                            messageLog.EmailTo = item.Emails;
                            messageLog.IsMailServer = true;
                            messageLog.EmailSubject = formatResult.Item1;
                            messageLog.EmailBody = formatResult.Item2;
                            messageLog.EnvironmentId = result.EnvironmentId;
                            messageLog.MachineName = result.MachineName;
                            messageLog.SendType = (int)SendType.Notification;
                            messageLog.Priority = (int)MessagePriority.Normal;
                            _notificationDataFacade.AddMessageLog(messageLog);
                        }
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError("ProcessFileTransferViolation For mail method : " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
                    }

                  
                }
            }
        }
    }
}

