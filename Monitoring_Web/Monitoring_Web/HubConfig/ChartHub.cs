using Microsoft.AspNetCore.SignalR;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring_Web.TimerFeatures;

namespace Monitoring_Web.HubConfig
{
    public interface IHubClient
    {
        Task TransferChartData(List<DashboardSystemHealthModel> Param);
    }
    public class ChartHub : Hub
    {
        private readonly TimerManager _timer;
        private readonly IDashboardService _dashboardService;
        private readonly IConfiguration _configuration;
        public ChartHub(TimerManager timer, IDashboardService dashboardService, IConfiguration configuration)
        {
            _timer = timer;
            _dashboardService = dashboardService;
            _configuration = configuration;
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (ConnectionInstance.ListConnection != null)
            {
                if (ConnectionInstance.ListConnection.Count == 1)
                {
                    ConnectionInstance.ListConnection.Clear();
                    _timer.IsTimerStarted = false;
                    _timer.Dispose();
                }
                else
                {
                    var item = ConnectionInstance.ListConnection.Find(a => a.ConnectionId == Context.ConnectionId);
                    if (item != null)
                    {
                        ConnectionInstance.ListConnection.Remove(item);
                    }
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public string GetConnectionId() => Context.ConnectionId;
    }
}
