using DiCentral.RetrySupport._6._0.ServiceHelper;
using Microsoft.Extensions.Logging;
using Monitoring.Data.IRepository;
using Monitoring.Data.Repository;
using Monitoring.Service.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Service.Services
{
    public class ReprocessService : IReprocessService
    {
        private readonly IReprocessRepository _reprocessRepository;
        private readonly ILogger<TransactionBaseRepository> _logger;
        public ReprocessService(IReprocessRepository reprocessRepository, ILogger<TransactionBaseRepository> logger)
        {
            this._reprocessRepository = reprocessRepository;
            this._logger = logger;
        }

        public async Task<string> CallReprocessAPI(string curl, string token, dynamic param)
        {
            string result = string.Empty;
            try
            {
                _logger.LogInformation("---CallReprocessAPI Token: " + token);
                string postbody = JsonConvert.SerializeObject(param);
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(curl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new System.Net.Http.StringContent(postbody, Encoding.UTF8, "application/json");
                    var response = await WebAPIHelper.Default.ExecuteAsync(() => client.PostAsync(curl, data), CancellationToken.None);
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
                _logger.LogError("---CallReprocessAPI ERROR----" + Convert.ToString(ex));
            }
            return result;
        }
    }
}
