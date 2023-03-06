using Monitoring.Model.Model;

namespace Monitoring_Web.HubConfig
{
    public static class ConnectionInstance
    {
        public static List<ConnectionInfo>? ListConnection { get; set; }
    }
    public class ConnectionInfo
    {
        public DashboardSystemHealthRequest? SystemHealthRequest { get; set; }
        public DashboardTransactionRequest? Transaction { get; set; }
        public int DashboardType { get; set; }
        public string ConnectionId { get; set; } = null!;
    }
}



