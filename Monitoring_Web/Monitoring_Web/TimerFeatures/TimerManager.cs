using Microsoft.AspNetCore.SignalR;
using Monitoring.Data.IRepository;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring.Service.Services;
using Monitoring_Common;
using Monitoring_Web.HubConfig;

namespace Monitoring_Web.TimerFeatures
{
    public class TimerManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChartHub> _hub;
        private readonly IServiceScope _scope;
        private readonly ILogger<TimerManager> _logger;

        public TimerManager(IConfiguration configuration, IHubContext<ChartHub> hub, IServiceProvider services, ILogger<TimerManager> logger)
        {
            _configuration = configuration;
            _hub = hub;
            _scope = services.CreateScope();
            _logger = logger;
        }

        private Timer? _timer;
        private AutoResetEvent? _autoResetEvent;
        private Action? _action;
        private int count = 0;
        public DateTime TimerStarted { get; set; }
        public bool IsTimerStarted { get; set; }
        private readonly object updateStockPricesLock = new object();
        private DashboardSystemHealthRequest _dashboardSystemHealthRequest;
        private DashboardTransactionRequest _dashboardTransactionRequest;
        public Task PrepareTimer(DashboardRequest dashboardRequest)
        {
            //_action = action;
            if (dashboardRequest.DashboardType == (int)Monitoring_Common.Enum.DashboardType.SystemHeath)
                _dashboardSystemHealthRequest = dashboardRequest.SystemHealthRequest ?? new DashboardSystemHealthRequest();
            else
                if (dashboardRequest.DashboardType == (int)Monitoring_Common.Enum.DashboardType.TransactionBased)
                _dashboardTransactionRequest = dashboardRequest.TransactionRequest ?? new DashboardTransactionRequest();
            _autoResetEvent = new AutoResetEvent(false);
            int interval = _configuration.GetValue<int>("IntervalDashboard");
            _timer = new Timer(Execute, _autoResetEvent, 1000, interval * 1000);
            TimerStarted = DateTime.Now;
            IsTimerStarted = true;
            return Task.CompletedTask;
        }
        public void Execute(object? stateInfo)
        {
            try
            {
                if (count > 0)
                {
                    var service = _scope.ServiceProvider.GetRequiredService<IDashboardService>();
                    for (int i = 0; i < HubConfig.ConnectionInstance.ListConnection?.Count; i++)
                    {
                        var item = ConnectionInstance.ListConnection[i];
                        var connectionId = item.ConnectionId;
                        if (item.DashboardType == (int)Monitoring_Common.Enum.DashboardType.SystemHeath)
                        {
                            var param = item.SystemHealthRequest;
                            int topSelect = _configuration.GetValue<int>("TopSelect");
                            if (param?.monitoringType == (int)Monitoring_Common.Enum.MonitoringType.FreeDisk)
                            {
                                var listdata = service.DataDashboardSystemHealth_KPIFreeDisk(param.nodeSettings, param.monitoringType, topSelect).Result;
                                _hub.Clients.Client(connectionId).SendAsync("TransferSystemHealthData", listdata).Wait();
                            }
                            else
                            {
                                var listdata = service.DataDashboardSystemHealth(param.nodeSettings, param.monitoringType, topSelect).Result;
                                _hub.Clients.Client(connectionId).SendAsync("TransferSystemHealthData", listdata).Wait();
                            }
                        }
                        else if (item.DashboardType == (int)Monitoring_Common.Enum.DashboardType.TransactionBased)
                        {
                            int topSelectTable = _configuration.GetSection("TransactionDashboard").GetValue<int>("TopSelectTransactionStatus");
                            int topSelectPendingGraph = _configuration.GetSection("TransactionDashboard").GetValue<int>("TopSelectPendingTransaction");
                            var paramRequest = item.Transaction;
                            int environmentID = paramRequest.EnvironmentID;
                            string cipFlow = paramRequest.CIPFlow;
                            string lastest = paramRequest.Lastest;
                            var dataOfTable = service.DashboardTransaction_Table(environmentID, cipFlow, lastest, topSelectTable).Result;
                            var dataOfColumnChart = service.DashboardTransaction_ColumnChart(environmentID, cipFlow, lastest).Result;
                            var dataOfPendingGraph = service.DashboardTransaction_PendingGraph(environmentID, cipFlow, lastest, topSelectPendingGraph).Result;
                            var thresholdPendingGraph = service.DashboardTransaction_ThresholdPendingGraph(environmentID).Result;
                            var returnData = new DashboardTransactionModel();
                            returnData.TableData = dataOfTable;
                            returnData.ColumnChartData = dataOfColumnChart;
                            returnData.PendingGraphData = dataOfPendingGraph;
                            returnData.ThresholdPendingGraph = thresholdPendingGraph;
                            _hub.Clients.Client(connectionId).SendAsync("TransferTransactionData", returnData).Wait();
                        }
                    }
                }
                count++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        }
        public void Dispose()
        {
            IsTimerStarted = false;
            if (_timer != null)
            {
                _timer.Dispose();
                count = 0;
            }
        }
    }
}
