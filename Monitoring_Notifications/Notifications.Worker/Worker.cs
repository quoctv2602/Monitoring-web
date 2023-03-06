using Notifications.Options;

namespace Monitoring_Notifications
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
        }
    }
}