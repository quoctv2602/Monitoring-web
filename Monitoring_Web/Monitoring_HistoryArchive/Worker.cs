using Monitoring_Common.DataContext;
using Monitoring_HistoryArchive.App_Code;
using Monitoring_wsGetHealth.App_Code;
using System.Runtime.InteropServices;

namespace Monitoring_HistoryArchive
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            _logger.LogInformation("Start");
            string StartTime = GlobalSettings.ProcessRunJobCron() + ":00";
            string EndTime = GlobalSettings.ProcessRunJobCron() + ":59";
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime StartDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + StartTime);
                DateTime EndDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + EndTime);
                DateTime currentDateTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
               
                if (currentDateTime == StartDateTime || (currentDateTime >= StartDateTime && currentDateTime <= EndDateTime))
                {
                    RunProcess();
                }
                await Task.Delay(1000*60, stoppingToken);
            }
        }
        private void RunProcess()
        {
            _logger.LogInformation("Start RunProcess method");
            try
            {
                string DayCount = GlobalSettings.DayCount();
                string dbConnect = GlobalSettings.ConnectionStrings();
                string query = string.Format(SQLStatements.MoveDataTableHistory, DayCount);
                int RowAction = SqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionStrings(), query);
                _logger.LogInformation("RowAction: " + RowAction);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message + "; - Stacktrace: " + ex.StackTrace);
            }

        }
    }
}