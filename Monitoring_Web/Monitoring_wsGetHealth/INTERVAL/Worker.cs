using Monitoring_Common.Security;
using Monitoring_wsGetHealth.App_Code;
using Monitoring_wsGetHealth.Repository;
using System.Data;
using System.Diagnostics;

namespace Monitoring_wsGetHealth.INTERVAL
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var tasks = new List<Task>()
                {
                    Transaction_System(stoppingToken),
                    GetListErrors(stoppingToken),
                    GetSummary(stoppingToken)
                };
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private  async Task Transaction_System(CancellationToken stoppingToken)
        {
            await Task.Yield();
            int ScheduleCronExpression = GlobalSettings.ScheduleCronExpression();
           
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running Transaction_System at: {time}", DateTimeOffset.Now);
                DataTable Config = Monitoring_EnvironmentConfigRepository.Monitoring_EnvironmentConfig();
                if (Config != null && Config.Rows.Count > 0)
                {
                    int count = 0;
                    RunProcess process = new RunProcess(_logger);
                    foreach (DataRow row in Config.Rows)
                    {
                        string EnvironmentID = Convert.ToString(row["EnvironmentID"]);
                        string Appid = Convert.ToString(row["Appid"]);
                        string HealthMeasurementKey = Convert.ToString(row["HealthMeasurementKey"]);
                        string Domain = Convert.ToString(row["domain_SystemHealth"]);
                        string MachineName = Convert.ToString(row["MachineName"]);
                        string ServiceList = Convert.ToString(row["ServiceList"]);
                        await process.Run(EnvironmentID, MachineName, ServiceList, Appid, HealthMeasurementKey, Domain).ConfigureAwait(false);
                        count++;
                    }
                    
                }
                await Task.Delay(ScheduleCronExpression).ConfigureAwait(false);
                // Task.Delay(ScheduleCronExpression, stoppingToken);
            }
        }

        /// <summary>
        /// Diconnect Get Summary Error
        /// </summary>
        /// <param name="stoppingToken"></param>
        private async Task GetSummary(CancellationToken stoppingToken)
        {
            await Task.Yield();
            int ScheduleCronExpression = GlobalSettings.ScheduleGetSummaryErrors();         
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running GetSummary at: {time}", DateTimeOffset.Now);
                DataTable Config = Monitoring_EnvironmentConfigRepository.Monitoring_EnvironmentConfig_DiConnect();
                if (Config != null && Config.Rows.Count > 0)
                {
                    int count = 0;
                    RunProcess process = new RunProcess(_logger);
                    foreach (DataRow row in Config.Rows)
                    {

                        string EnvironmentID = Convert.ToString(row["EnvironmentID"]);
                        string Appid = Convert.ToString(row["Appid"]);
                        string AppKey = Convert.ToString(row["HealthMeasurementKey"]);
                        string Domain = Convert.ToString(row["domain_SystemHealth"]);
                        await process.Run_Diconnect("GetSummary", EnvironmentID, Appid, AppKey, Domain, "I").ConfigureAwait(false);
                        await process.Run_Diconnect("GetSummary", EnvironmentID, Appid, AppKey, Domain, "O").ConfigureAwait(false);
                        count++;
                    }

                }
                await Task.Delay(ScheduleCronExpression).ConfigureAwait(false);
                //Task.Delay(ScheduleCronExpression, stoppingToken);
            }
        }

        /// <summary>
        /// DiConnect get list error transaction
        /// </summary>
        /// <param name="stoppingToken"></param>

        private async Task GetListErrors(CancellationToken stoppingToken)
        {
            await Task.Yield();
            int ScheduleCronExpression = GlobalSettings.ScheduleGetTopErrors();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running GetListErrors at: {time}", DateTimeOffset.Now);
                DataTable Config = Monitoring_EnvironmentConfigRepository.Monitoring_EnvironmentConfig_DiConnect();
                if (Config != null && Config.Rows.Count > 0)
                {
                    int count = 0;
                    RunProcess process = new RunProcess(_logger);
                    foreach (DataRow row in Config.Rows)
                    {
                        string EnvironmentID = Convert.ToString(row["EnvironmentID"]);
                        string Appid = Convert.ToString(row["Appid"]);
                        string AppKey = Convert.ToString(row["HealthMeasurementKey"]);
                        string Domain = Convert.ToString(row["domain_SystemHealth"]);                         
                        _logger.LogInformation("ExecuteAsync GetListErrors: {0}", Domain);                            
                        await process.Run_Diconnect("GetListErrors",EnvironmentID, Appid, AppKey, Domain,"I").ConfigureAwait(false);
                        await process.Run_Diconnect("GetListErrors", EnvironmentID, Appid, AppKey, Domain,"O").ConfigureAwait(false);
                        count++;
                    }
                }
                await Task.Delay(ScheduleCronExpression).ConfigureAwait(false);
            }
        }
    }
}