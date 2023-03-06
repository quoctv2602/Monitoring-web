using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Notifications.DAL.EFModel;
using Notifications.Options;
using System;
using System.Linq.Expressions;
using Monitoring_Common.DataContext;
using DiCentral.RetrySupport._6._0.DBHelper;

namespace Notifications.DAL
{
    public class NotificationDataFacade : INotificationDataFacade
    {
        private readonly Configs _configs;
        private readonly ILogger _logger;
        public NotificationDataFacade(ILogger<NotificationDataFacade> logger, Configs configs)
        {
            _configs = configs;
            _logger = logger;
        }
        public EmailConfig GetEmailServer(int environmentId)
        {
            //_logger.LogInformation("Start GetEmailServer method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetEmailServers, environmentId);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    SysEmailServer? sysEmailServer = DBRetryHelper.Default.Execute(() => dbContext.SysEmailServer.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (sysEmailServer == null)
                        return new EmailConfig();
                    return ConvertSysEmailServerToEmailConfig(sysEmailServer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetEmailServer method");
            return new EmailConfig();
        }
        public SysNodeSetting GetNodeSettings(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetNodeSettings method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetNodeSettings, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    SysNodeSetting? item = DBRetryHelper.Default.Execute(() => dbContext.SysNodeSetting.FromSqlRaw(sqlStr).FirstOrDefault());
                    if (item == null)
                        return new SysNodeSetting();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetNodeSettings method");
            return new SysNodeSetting();
        }
        public SysNodeSetting GetNodeSettings(int environmentId)
        {
            //_logger.LogInformation("Start GetNodeSettings method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetNodeTransactionSettings, environmentId);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    SysNodeSetting? item = DBRetryHelper.Default.Execute(() => dbContext.SysNodeSetting.FromSqlRaw(sqlStr).FirstOrDefault());
                    if (item == null)
                        return new SysNodeSetting();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetNodeSettings method");
            return new SysNodeSetting();
        }
        public SysNodeSetting GetNodeSettings(int environmentId, int NodeType)
        {
            //_logger.LogInformation("Start GetNodeSettings method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetNodeSettingsByNodeType, environmentId, NodeType);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    SysNodeSetting? item = DBRetryHelper.Default.Execute(() => dbContext.SysNodeSetting.FromSqlRaw(sqlStr).FirstOrDefault());
                    if (item == null)
                        return new SysNodeSetting();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetNodeSettings method");
            return new SysNodeSetting();
        }
        public List<SysNodeSetting> GetNodeList()
        {
            //_logger.LogInformation("Start GetNodeList method");
            try
            {
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    return DBRetryHelper.Default.Execute(() => dbContext.SysNodeSetting.FromSqlRaw(SQLStatements.GetNodeList).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetNodeList method");
            return new List<SysNodeSetting>();
        }
        public SysEnvironment GetEnvironment(int environmentId)
        {
            //_logger.LogInformation("Start GetEnvironment method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetEnvironments, environmentId);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    SysEnvironment? item = DBRetryHelper.Default.Execute(() => dbContext.SysEnvironment.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (item == null)
                        return new SysEnvironment();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetEnvironment method");
            return new SysEnvironment();
        }

        public SysMonitoring GetMonitoring(int id)
        {
            //_logger.LogInformation("Start GetMonitoring method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetMonitorings, id);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    SysMonitoring item = DBRetryHelper.Default.Execute(() => dbContext.SysMonitoring.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (item == null)
                        return new SysMonitoring();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetEnvironment method");
            return new SysMonitoring();
        }

        public List<SysNotificationDetail> GetListMail(int KPIid)
        {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetListMailNotification, KPIid);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    return DBRetryHelper.Default.Execute(() => dbContext.SysNotificationDetail.FromSqlRaw(sqlStr).ToList());
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            return new List<SysNotificationDetail>();
        }
        public List<SysThresholdRule> GetThresholdRules(int monitoringType)
        {
            //_logger.LogInformation("Start GetThresholdRules method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetThresholdRules, monitoringType);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    return DBRetryHelper.Default.Execute(() => dbContext.SysThresholdRule.FromSqlRaw(sqlStr).ToList());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetThresholdRules method");
            return new List<SysThresholdRule>();
        }
        public TransSystemHealthDTO GetViolatedRecordsByCPU(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords)
        {
            //_logger.LogInformation("Start GetViolatedRecordsByCPU method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByCPU,
                        hours, statusBeforeStr, environmentId, machineName,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetViolatedRecordsByCPU method");
            return new TransSystemHealthDTO();
        }

        public TransSystemHealthDTO GetViolatedRecordsByErrorNumbers(
                       string statusBeforeStr, int environmentId,
                           int? thresholdCount, int statusAfter, int maxRecords)
        {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByErrorNumbers,
                       statusBeforeStr, environmentId,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            return new TransSystemHealthDTO();
        }
        public TransSystemHealthDTO GetViolatedRecordsByIntergrationErrorNumbers(
                      string statusBeforeStr, int environmentId,
                          int? thresholdCount, int statusAfter, int maxRecords)
        {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByIntergrationErrorNumbers,
                        statusBeforeStr, environmentId,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            return new TransSystemHealthDTO();
        }
        public TransSystemHealthDTO GetViolatedRecordsByPendingTransactions(
                      string statusBeforeStr, int environmentId,
                          int? thresholdCount, int statusAfter, int maxRecords)
        {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByPendingTransactions,
                         statusBeforeStr, environmentId,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            return new TransSystemHealthDTO();
        }







        public TransSystemHealthDTO GetViolatedRecordsByFreeDisk(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords)
        {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByFreeDisk,
                        hours, statusBeforeStr, environmentId, machineName,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetViolatedRecordsByCPU method");
            return new TransSystemHealthDTO();
        }
        public TransSystemHealthDTO GetViolatedRecordsByTransaction(int hours,
                       string statusBeforeStr, int environmentId, string machineName,
                           int? thresholdCount, int statusAfter, int maxRecords)
        {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByTransaction,
                        hours, statusBeforeStr, environmentId, machineName,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetViolatedRecordsByCPU method");
            return new TransSystemHealthDTO();
        }
        public TransSystemHealthDTO GetViolatedRecordsByFileTransfer(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords)
        {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByFileTransfer,
                        hours, statusBeforeStr, environmentId, machineName,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetViolatedRecordsByCPU method");
            return new TransSystemHealthDTO();
        }


        public TransSystemHealthDTO GetViolatedRecordsByMemory(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords)
        {
            //_logger.LogInformation("Start GetViolatedRecordsByMemory method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByMemory,
                        hours, statusBeforeStr, environmentId, machineName,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetViolatedRecordsByMemory method");

            return new TransSystemHealthDTO();
        }
        public TransSystemHealthDTO GetViolatedRecordsByStorage(int hours,
                        string statusBeforeStr, int environmentId, string machineName,
                            int? thresholdCount, int statusAfter, int maxRecords)
        {
            //_logger.LogInformation("Start GetViolatedRecordsByStorage method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateViolatedRecordsByStorage,
                        hours, statusBeforeStr, environmentId, machineName,
                        thresholdCount, statusAfter, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr)
                        .ToList().Distinct().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetViolatedRecordsByStorage method");

            return new TransSystemHealthDTO();
        }
        public TransSystemHealthDTO GetSystemError(int environmentId, string machineName, int maxRecords)
        {
            //_logger.LogInformation("Start GetSystemError method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateSystemError, environmentId, machineName, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthDTO? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthDTO.FromSqlRaw(sqlStr).AsEnumerable().SingleOrDefault());
                    if (item == null)
                        return new TransSystemHealthDTO();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetSystemError method");

            return new TransSystemHealthDTO();
        }

        public TransSystemHealthProcessService GetServiceError(int environmentId, string machineName, int maxRecords)
        {
            //_logger.LogInformation("Start GetSystemError method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetAndUpdateServiceError, environmentId, machineName, maxRecords);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    TransSystemHealthProcessService? item = DBRetryHelper.Default.Execute(() => dbContext.TransSystemHealthProcessService.FromSqlRaw(sqlStr).AsEnumerable().FirstOrDefault());
                    if (item == null)
                        return new TransSystemHealthProcessService();
                    return item;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetSystemError method");

            return new TransSystemHealthProcessService();
        }
        public void AddMessageLog(TransMessageLog message)
        {
            //_logger.LogInformation("Start AddMessageLog method");
            try
            {
                string sqlStr = string.Format(SQLStatements.SaveToMessageLog, message.Status, message.CreatedBy,
                            message.EmailTo, 1, message.EmailSubject, message.EmailBody.Replace("'", "\""),
                            message.MachineName, message.EnvironmentId, message.SendType, message.Priority);

                SqlHelper.ExecuteNonQuery(_configs.DatabaseSettings.NotificationContext, sqlStr);
                //using (var dbContext = new NotificationContext(_logger, _configs))
                //{
                //    dbContext.Database.ExecuteSqlRaw(sqlStr);
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End AddMessageLog method");

        }

        public List<TransMessageLog> GetDailyReportLogs(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetDailyReportLogs method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetDailyReportLogs, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    return DBRetryHelper.Default.Execute(() => dbContext.TransMessageLog.FromSqlRaw(sqlStr)
                        .ToList());

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetDailyReportLogs method");

            return new List<TransMessageLog>();
        }
        public StatisticsDTO GetStatisticsByCPU(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetDailyReportLogs method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatisticsByCPU, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetDailyReportLogs method");

            return new StatisticsDTO();
        }
        public StatisticsDTO GetStatistbyTransaction(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetDailyReportLogs method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatistbyTransaction, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetDailyReportLogs method");

            return new StatisticsDTO();
        }
        public StatisticsDTO GetStatistbyFileTransfer(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetDailyReportLogs method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatistbyFileTransfer, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetDailyReportLogs method");

            return new StatisticsDTO();
        }
        public List<StatisticsDTO_FreeDisk> GetStatisticsByFreeDisk(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetDailyReportLogs method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatistbyFreeDisk, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    List<StatisticsDTO_FreeDisk> ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO_FreeDisk.FromSqlRaw(sqlStr).ToList());
                    if (ret == null)
                        return new List<StatisticsDTO_FreeDisk>();
                    return ret;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetDailyReportLogs method");

            return new List<StatisticsDTO_FreeDisk>();
        }
        public StatisticsDTO GetStatisticsByMemory(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetDailyReportLogs method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatisticsByMemory, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetDailyReportLogs method");

            return new StatisticsDTO();
        }
        public StatisticsDTO GetStatisticsByStorage(int environmentId, string machineName)
        {
            //_logger.LogInformation("Start GetDailyReportLogs method");
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatisticsByStorage, environmentId, machineName);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            //_logger.LogInformation("End GetDailyReportLogs method");

            return new StatisticsDTO();
        }

        public StatisticsDTO GetStatisticsByErrorNumbers(int environmentId) {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatisticsByErrorNumbers, environmentId);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }

            return new StatisticsDTO();
        }
        public StatisticsDTO GetStatisticsByIntergrationErrorNumbers(int environmentId) {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatisticsByIntergrationErrorNumbers, environmentId);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }

            return new StatisticsDTO();
        }
        public StatisticsDTO GetStatisticsByPendingTransactions(int environmentId) {
            try
            {
                string sqlStr = string.Format(SQLStatements.GetStatisticsByPendingTransactions, environmentId);
                using (var dbContext = new NotificationContext(_logger, _configs))
                {
                    StatisticsDTO ret = DBRetryHelper.Default.Execute(() => dbContext.StatisticsDTO.FromSqlRaw(sqlStr).SingleOrDefault());
                    if (ret == null)
                        return new StatisticsDTO();
                    return ret;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }

            return new StatisticsDTO();
        }



        private EmailConfig ConvertSysEmailServerToEmailConfig(SysEmailServer sysEmailServer)
        {
            var config = new EmailConfig();
            config.FromEmail = sysEmailServer.FromEmail;
            config.EnableSSL = sysEmailServer.EnableSsl;
            config.SmtpUser = sysEmailServer.UserName;
            config.SmtpPassword = sysEmailServer.Password;
            config.SmtpServer = sysEmailServer.SmtpServer;
            config.SmtpPort = sysEmailServer.Port;
            return config;
        }
    }
}
