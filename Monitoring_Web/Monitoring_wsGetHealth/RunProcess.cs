using DiCentral.RetrySupport._6._0.ServiceHelper;
using Monitoring_Common.Security;
using Monitoring_wsGetHealth.App_Code;
using Monitoring_wsGetHealth.INTERVAL;
using Monitoring_wsGetHealth.Model;
using Monitoring_wsGetHealth.Repository;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Monitoring_wsGetHealth
{

    public class RunProcess
    {
        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();

        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //const int SW_HIDE = 0;
        //private static string EnvironmentID;
        //private static string ServiceList;
        //private static string Appid;
        //private static string HealthMeasurementKey;
        //private static string Domain;
        //private static string MachineName;

        private readonly ILogger<Worker> _logger;


        public RunProcess(ILogger<Worker> logger)
        {
            //EnvironmentID = _environmentID;
            //Appid = _appId;
            //HealthMeasurementKey = _healthMeasurementKey;
            //Domain = domain_SystemHealth;
            //MachineName = _machineName;
            //ServiceList = _ServiceList;
            _logger = logger;
            // tan adsd
            //var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE);
        }

        public async Task Run(string EnvironmentID, string MachineName, string ServiceList, string Appid, string HealthMeasurementKey, string Domain)
        {

            _logger.LogInformation("Start");
            string token = AES.EncryptAES(Appid, HealthMeasurementKey);
            string CreateRequestID = "";
            MonitoringRespone counter = new MonitoringRespone();
            if (!string.IsNullOrEmpty(token) && token != "Error")
            {
                CreateRequestID = Monitoring_SystemRepository.CreateRequestID(Convert.ToInt32(EnvironmentID), MachineName);
                counter = await GetDataSystemHealth(token, CreateRequestID, ServiceList, Domain);
            }

            if (counter != null)
            {
                try
                {
                    _logger.LogInformation("Run.insert db");
                    if (counter.Status == 0)
                    {
                        int fail = Monitoring_SystemRepository.ImportMonitoring_System_Fail(CreateRequestID, counter.Message);
                    }
                    else
                    {
                        int import = Monitoring_SystemRepository.ImportMonitoring_System(CreateRequestID, Convert.ToInt32(EnvironmentID), MachineName, counter.result.IpAddress, counter.result.CPUInfo, counter.result.MemoryInfo, counter.requestTime, counter.responseTime, counter.result.StorageInfo, 0, counter.contentData, counter.Status, counter.Message, counter.detail, counter.disk, counter.Transfer, counter.EDItoASCII);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError("Import db Error " + ex.Message);
                }

            }
        }

        public async Task Run_Diconnect(string Process, string EnvironmentID, string Appid, string AppKey, string Domain, string CIPFlow)
        {
            _logger.LogInformation("Start");
            string token = AES.EncryptAES(Appid, AppKey);
            try
            {

                if (!string.IsNullOrEmpty(token) && token != "Error")
                {
                    string StartDate = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00.000";
                    string EndDate = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59.999";
                 
                    if (Process == "GetSummary")
                    {
                        string CreateRequestID = Monitoring_TransactionRepository.CreateRequestID(Process, EnvironmentID);

                        await GetSummary(token, CreateRequestID, Domain, StartDate, EndDate, CIPFlow, EnvironmentID);
                      
                    }
                    if (Process == "GetListErrors")
                    {
                       
                        await GetListError(token, Guid.NewGuid().ToString(), Domain, StartDate, EndDate, CIPFlow, EnvironmentID);
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("Run_Diconnect " + Process + ": " + ex.Message);
            }
        }

        public async Task RunDaily(string Appid, string HealthMeasurementKey)
        {
            try
            {
                _logger.LogInformation("Start");
                string token = AES.EncryptAES(Appid, HealthMeasurementKey);
            }
            catch (Exception ex)
            {

                _logger.LogError("RunDaily() " + ex.ToString());
            }
        }

        public async Task<MonitoringRespone> GetDataSystemHealth(string token, string UUID, string ServiceList, string Domain)
        {
            MonitoringRespone counter = new MonitoringRespone();
            counter.Status = 0;
            try
            {
                string requestTime = DateTime.Now.ToString();
                //helper.WriteFileLog("GetDataSystemHealth");
                _logger.LogInformation("GetDataSystemHealth");
                string sResponseFromServer = "";
                string curl = Domain + "api/HealthMeasurement/getMonitor?UUID=" + UUID + "&ServiceList=" + ServiceList;
                string postbody = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(curl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new System.Net.Http.StringContent(postbody, Encoding.UTF8, "application/json");
                    var response = await WebAPIHelper.Default.ExecuteAsync(() => client.PostAsync(curl, data), CancellationToken.None);
                    sResponseFromServer = await response.Content.ReadAsStringAsync();
                    counter = JsonConvert.DeserializeObject<MonitoringRespone>(sResponseFromServer);
                    counter.contentData = sResponseFromServer;
                    counter.responseTime = DateTime.Now.ToString();
                    counter.requestTime = requestTime;
                    counter.Status = 1;
                }
                return counter;

            }
            catch (Exception ex)
            {
                counter.Status = 0;
                counter.Message = ex.Message;
                //helper.WriteFileLog("GetDataSystemHealth Error " + ex.ToString());
                _logger.LogError("GetDataSystemHealth Error " + ex.ToString());
                return counter;
            }
        }

        public async Task GetSummary(string token, string UUID, string Domain, string StartDate, string EndDate, string CIPFlow, string EnvironmentID)
        {
            TransactionBaseModel Response = new TransactionBaseModel();
            string RequestTime = DateTime.Now.ToString();
            string ResponseTime;
            string result = "";
            try
            {
                string requestTime = DateTime.Now.ToString();
                _logger.LogInformation("GetSummary");

                string curl = string.Format(Domain + "api/Monitoring/GetSummaryByStatus?RequestID={0}&StartDate={1}&EndDate={2}&CIPFlow={3}", UUID, StartDate, EndDate, CIPFlow);
                string postbody = "";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(curl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new System.Net.Http.StringContent(postbody, Encoding.UTF8, "application/json");
                    var sResponse = await WebAPIHelper.Default.ExecuteAsync(() => client.PostAsync(curl, data), CancellationToken.None);
                    result = await sResponse.Content.ReadAsStringAsync();
                    Response = JsonConvert.DeserializeObject<TransactionBaseModel>(result);
                }
              
                ResponseTime = DateTime.Now.ToString();
                _logger.LogInformation("Insert db data GetSummary");
                try
                {
                    Monitoring_TransactionRepository.UpdateDataSummary(RequestTime, ResponseTime, CIPFlow, StartDate, EndDate, result, Response.ErrorNumbers.ToString(), Response.IntergrationErrorNumbers.ToString(), Response.PendingNumbers.ToString(), "1", null, UUID, EnvironmentID);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Insert db data GetSummary " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GetSummary Error " + ex.Message);
                ResponseTime = DateTime.Now.ToString();
                try
                {
                    Monitoring_TransactionRepository.UpdateDataSummary_Fail(RequestTime, ResponseTime, CIPFlow, StartDate, EndDate, result, "0", ex.Message, UUID);
                }
                catch (Exception e)
                {

                    _logger.LogError("Insert db data GetSummary " + e.Message);
                }

            }

        }


        public async Task GetListError(string token, string UUID, string Domain, string StartDate, string EndDate, string CIPFlow, string EnvironmentID)
        {

            TransactionBaseListError Response = new TransactionBaseListError();
            string RequestTime = DateTime.Now.ToString();
            string ResponseTime;
            string sResponseFromServer = "";
            try
            {
                string requestTime = DateTime.Now.ToString();
                _logger.LogInformation("GetListError");
                string TopList = GlobalSettings.TopErrors().ToString();
                string postbody = "";
                string curl = string.Format(Domain + "api/Monitoring/GetTopErrors?RequestID={0}&StartDate={1}&EndDate={2}&CIPFlow={3}&NumberOfTransactions={4}", UUID, StartDate, EndDate, CIPFlow, TopList);

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(curl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new System.Net.Http.StringContent(postbody, Encoding.UTF8, "application/json");
                    var sResponse = await WebAPIHelper.Default.ExecuteAsync(() => client.PostAsync(curl, data), CancellationToken.None);
                    var result = await sResponse.Content.ReadAsStringAsync();
                    Response = JsonConvert.DeserializeObject<TransactionBaseListError>(result);
                }

                ResponseTime = DateTime.Now.ToString();
                _logger.LogInformation("Insert db data GetListError");
                try
                {
                   
                    Monitoring_TransactionRepository.ImportDataListError(RequestTime, ResponseTime, CIPFlow, StartDate, EndDate, sResponseFromServer, "1", null, UUID, Response, EnvironmentID);
                }
                catch (Exception ex)
                {

                    _logger.LogError("Insert db data GetListError " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GetListError Error " + ex.Message);
                ResponseTime = DateTime.Now.ToString();
                try
                {
                    Monitoring_TransactionRepository.ImportDataListError(RequestTime, ResponseTime, CIPFlow, StartDate, EndDate, sResponseFromServer, "0", ex.Message, UUID, Response, EnvironmentID);
                }
                catch (Exception e)
                {

                    _logger.LogError("Insert db data GetListError " + e.Message);
                }
            }

        }

    }
}
