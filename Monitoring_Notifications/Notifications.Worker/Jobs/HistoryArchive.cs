using Monitoring_Common.DataContext;
using Notifications.DAL;
using Notifications.EmailService;
using Notifications.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Monitoring_Notifications.Jobs
{
    public class HistoryArchive : IJob
    {
        private readonly ILogger _logger;
        private readonly Configs _configs;
        public HistoryArchive(ILogger<HistoryArchive> logger, Configs configs)
        {
            _logger = logger;

            
              _configs = configs;
        }
        public Task Execute(IJobExecutionContext context) {
            _logger.LogInformation("Archive Job started! - " + DateTime.Now.ToString());
            try
            {
                ProcessHistoryArchive();
            }
            catch (Exception ex)
            {
                _logger.LogError("Archive Job! - " + ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }
            _logger.LogInformation("Archive Job ended! - " + DateTime.Now.ToString());
            return Task.CompletedTask;
        }
        private void ProcessHistoryArchive()
        {
            _logger.LogInformation("Start RunProcess method");
            try
            {
                string DayCount = _configs.AppSettings.DayCount;
                string dbConnect = _configs.DatabaseSettings.NotificationContext;
                string query = string.Format(SQLStatements.MoveDataTableHistory, DayCount);
                int RowAction = SqlHelper.ExecuteNonQuery(dbConnect, query);
                _logger.LogInformation("RowAction: " + RowAction);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }

        }
    }
}
