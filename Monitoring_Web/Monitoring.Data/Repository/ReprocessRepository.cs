using DiCentral.RetrySupport._6._0.ServiceHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Monitoring.Data.IRepository;
using Monitoring.Model.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data.Repository
{
    public class ReprocessRepository: IReprocessRepository
    {
        private static string _dbConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MonitoringTool"];

        private readonly MonitoringContext _monitoringContext;
        private readonly ILogger<ReprocessRepository> _logger;
        public ReprocessRepository(MonitoringContext monitoringContext, ILogger<ReprocessRepository> logger)
        {
            _monitoringContext = monitoringContext;
            _logger = logger;
        }
        

        public Task<ResponseData> UpdateReprocessStatus(ReprocessModel param, int EnvironmentID)
        {
            throw new NotImplementedException();
        }
    }
}
