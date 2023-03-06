using Monitoring_wsGetHealth.App_Code;
using Monitoring_wsGetHealth.INTERVAL;
using Monitoring_wsGetHealth.Repository;
using System.Data;

namespace Monitoring_wsGetHealth.DAILY
{
    public class WorkerDaily : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public WorkerDaily(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        private Timer Schedular;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int ScheduleCronExpression = GlobalSettings.ScheduleCronExpression();
           
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                  
                    string scheduledTime ="";
                    scheduledTime = DateTime.Parse(GlobalSettings.ScheduledTimeDaily()).ToString("yyyy-MM-dd HH:mm");
                    if (DateTime.Now.ToString("yyyy-MM-dd HH:mm") == scheduledTime)
                    {
                        DataTable Config = Monitoring_EnvironmentConfigRepository.Monitoring_EnvironmentConfig();


                        for (int i = 0; i < Config.Rows.Count; i++)
                        {
                            try
                            {
                                int count = i;
                                string EnvironmentID = Convert.ToString(Config.Rows[i]["EnvironmentID"]);
                                string Appid = Convert.ToString(Config.Rows[i]["Appid"]);
                                string HealthMeasurementKey = Convert.ToString(Config.Rows[i]["HealthMeasurementKey"]);
                                string Domain = Convert.ToString(Config.Rows[i]["domain_SystemHealth"]);
                                string MachineName = Convert.ToString(Config.Rows[i]["MachineName"]);
                                string ServiceList = Convert.ToString(Config.Rows[i]["ServiceList"]);



                                //   helper.WriteFileLog("ExecuteAsync EnvironmentID " + EnvironmentID);
                                _logger.LogInformation("ExecuteAsync EnvironmentID:{0} - Count: {1} ", EnvironmentID, count);
                                RunProcess process = new RunProcess(_logger);
                                await process.RunDaily(Appid, HealthMeasurementKey);
                                await Task.Delay(ScheduleCronExpression).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {

                                // helper.WriteFileLog("ExecuteAsync Error " + ex.ToString());
                                _logger.LogError("ExecuteAsync Error " + ex.ToString());
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError("ExecuteAsync Error " + ex.ToString());
                }
                await Task.Delay(60*1000, stoppingToken);
            }

        }
    
    }
}
