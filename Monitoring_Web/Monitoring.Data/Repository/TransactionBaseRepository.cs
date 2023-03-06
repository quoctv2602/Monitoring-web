using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring_Common.DataContext;
using Monitoring_Common.Security;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using DiCentral.RetrySupport._6._0.DBHelper;
using DiCentral.RetrySupport._6._0.ServiceHelper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Monitoring.Data.Extensions;
using static Monitoring_Common.Enum;

namespace Monitoring.Data.Repository
{
    public class TransactionBaseRepository : ITransactionBaseRepository
    {
        private static string _dbConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["MonitoringTool"];

        private readonly MonitoringContext _monitoringContext;
        private readonly ILogger<TransactionBaseRepository> _logger;
        public TransactionBaseRepository(MonitoringContext monitoringContext, ILogger<TransactionBaseRepository> logger)
        {
            _monitoringContext = monitoringContext;
            _logger = logger;
        }

        public async Task<ResponseModel<string>> CallDataDIConnect(string curl, string token, dynamic param)
        {
            var result = new ResponseModel<string>();
            try
            {
                _logger.LogInformation("---token----" + token);
                string postbody = JsonConvert.SerializeObject(param);
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(curl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", token);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var data = new System.Net.Http.StringContent(postbody, Encoding.UTF8, "application/json");
                    var response = await WebAPIHelper.Default.ExecuteAsync(() => client.PostAsync(curl, data), CancellationToken.None);
                    var Content = await response.Content.ReadAsStringAsync();
                    result.ResponseStatus = (int)ResponseStatus.Success;
                    result.Data = Content;
                }
            }
            catch (Exception ex)
            {
                result.ResponseStatus = (int)ResponseStatus.Error;
                result.Description = ex.Message;
                _logger.LogError("---CallDataDIConnect ERROR----" + Convert.ToString(ex));
            }
            return result;
        }

        public async Task<List<TransDataIntergrationMappedModel>> GetCIPReporting(CIPReportingModel param, int EnvironmentID)
        {
            /*
             Step 0: Get Info DIConnect
             Step 1: Create requestID insert into db table Log (starttime)
             Step 2: Call API DIConnect "GET CIP Reporting"
             Step 3: Update Status RequestId = 1 (success) and endtime request
             Step 4: Get Master data Monitoring Mapping data Reponse "GET CIP Reporting"
             Step 5: return data for web
             */
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            Guid CreateRequestID = Guid.NewGuid();
            List<ListTransactionErrors> Response = new List<ListTransactionErrors>();
            try
            {
                _logger.LogInformation("---Begin GetInfoEnvironment in TransactionBaseRepository, ConnectionString: " + _dbConnectionString);
                IntegrationAPIModel integrationAPI = await GetTransactionIntegrationAPI(EnvironmentID);
                if (integrationAPI != null)
                {
                    string URL = Convert.ToString(integrationAPI.domain_SystemHealth);
                    string Appid = Convert.ToString(integrationAPI.Appid);
                    string AppKey = Convert.ToString(integrationAPI.HealthMeasurementKey);

                    _logger.LogInformation("---Appid----" + Appid);
                    _logger.LogInformation("---AppKey----" + AppKey);
                    string token = AES.EncryptAES(Appid, AppKey);
                    _logger.LogInformation("---token----" + token);
                    string curl = URL + "api/Monitoring/GetCIPReporting";

                    _logger.LogInformation("---Begin CreateRequestID in TransactionBaseRepository---");
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, null, "", null, null);

                    _logger.LogInformation("---End CreateRequestID in TransactionBaseRepository, RequestID: " + CreateRequestID);

                    StartTime = DateTime.Now;
                    curl = curl + "?requestID=" + CreateRequestID;
                    _logger.LogInformation("---Begin CallDataDIConnect in TransactionBaseRepository---" + "URL:" + curl + ",Token:" + token + ",Param:" + JsonConvert.SerializeObject(param));
                    var dataresponse = await CallDataDIConnect(curl, token, param);
                    // _logger.LogInformation("---Response dataresponse --- : " + dataresponse);
                    EndTime = DateTime.Now;
                    try
                    {
                        if (dataresponse.ResponseStatus== (int)ResponseStatus.Success)
                        {
                            Response = JsonConvert.DeserializeObject<List<ListTransactionErrors>>(dataresponse.Data);

                            if (Response != null && Response.Count > 0)
                            {
                                string queryMapping = string.Empty;
                                queryMapping = @"DECLARE @ResponeDIConnect AS TABLE (RowID BIGINT,ErrorStatus INT)";
                                string detail = "";
                                for (int i = 0; i < Response.Count; i++)
                                {
                                    var item = Response[i];
                                    if (item.serverFileId != null)
                                        detail += "(" + ReturnValue(item.serverFileId) + "," + ReturnValue(item.ErrorStatus) + "),";
                                }
                                queryMapping += String.Format(@"INSERT INTO @ResponeDIConnect VALUES {0}", detail.Substring(0, detail.Length - 1));
                                queryMapping += @"
                                    SELECT r.RowID,tdi.MonitoredStatus,tdi.Note,tdi.ReProcess,sem.Name EnvironmentName,es.Description ErrorStatusString
                                    FROM @ResponeDIConnect AS r
                                    LEFT JOIN Trans_Data_Integration AS tdi WITH (NOLOCK) ON r.RowID=tdi.RowID
                                    LEFT JOIN Sys_ErrorStatus AS es WITH (NOLOCK) ON r.ErrorStatus=es.ErrorStatus
                                    LEFT JOIN Sys_Environment AS sem WITH (NOLOCK) ON sem.ID=" + EnvironmentID;
                                var baseMapping = await _monitoringContext.BaseTransDataIntergrationMapped.FromSqlRaw(queryMapping).ToListAsync();
                                var returnMapping = new List<TransDataIntergrationMappedModel>();
                                foreach (var responeItem in Response)
                                {
                                    var rowID = responeItem.serverFileId;
                                    var findItem = baseMapping.FirstOrDefault(a => a.RowID == rowID);
                                    if (findItem != null)
                                    {
                                        var itemReturn = new TransDataIntergrationMappedModel();
                                        itemReturn.Direction = responeItem.Direction;
                                        itemReturn.MonitoredStatus = findItem.MonitoredStatus;
                                        itemReturn.RowsNumber = responeItem.RowsNumber;
                                        itemReturn.senderCustId = responeItem.senderCustId;
                                        itemReturn.SenderCustName = responeItem.SenderCustName;
                                        itemReturn.Document = responeItem.Document;
                                        itemReturn.ReProcess = findItem.ReProcess;
                                        itemReturn.EnvironmentID = EnvironmentID;
                                        itemReturn.EnvironmentName = findItem.EnvironmentName;
                                        itemReturn.DocType = responeItem.DocType;
                                        itemReturn.StartDate = responeItem.StartDate;
                                        itemReturn.EndDate = responeItem.EndDate;
                                        itemReturn.ErrorStatus = responeItem.ErrorStatus;
                                        itemReturn.Note = findItem.Note;
                                        itemReturn.receiverCustId = responeItem.receiverCustId;
                                        itemReturn.ReceiverCustName = responeItem.ReceiverCustName;
                                        itemReturn.TotalOfDocs = responeItem.TotalOfDocs;
                                        itemReturn.TotalRows = responeItem.TotalRows;
                                        itemReturn.TransactionKey = responeItem.TransactionKey;
                                        itemReturn.RowID = responeItem.serverFileId;
                                        itemReturn.ErrorStatusString = findItem.ErrorStatusString;
                                        returnMapping.Add(itemReturn);
                                    }
                                }
                                await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Succeeded, "", StartTime, EndTime);
                                return returnMapping;
                            }
                            await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Succeeded, "", StartTime, EndTime);
                        }
                        else
                        {
                            _logger.LogInformation("---Response dataresponse IsNullOrEmpty---");
                            await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, dataresponse.Description, StartTime, EndTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        EndTime = DateTime.Now;
                        await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, "", (int)TransactionLogStatus.Failed, ex.Message, StartTime, EndTime);
                        _logger.LogInformation("---Exception JsonConvert.DeserializeObject---" + ex.Message);

                    }                    
                }
                else
                {
                    _logger.LogInformation("---reader is null in TransactionBaseRepository---");
                    return new List<TransDataIntergrationMappedModel>();
                }
            }
            catch (Exception ex)
            {
                EndTime = DateTime.Now;
                _logger.LogInformation("----End method GetCIPReporting---");
                _logger.LogError(ex.Message);
                await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, "", (int)TransactionLogStatus.Failed, ex.Message, StartTime, EndTime);
                return new List<TransDataIntergrationMappedModel>();

            }
            return new List<TransDataIntergrationMappedModel>();
        }
        public async Task<ResponseData> GetReportByTransactionKey(ReportbyTransactionModel param, int EnvironmentID)
        {
            ResponseData responseData = new ResponseData();
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            Guid CreateRequestID = Guid.NewGuid();
            string curl = "";
            try
            {
                
                IntegrationAPIModel integrationAPI = await GetTransactionIntegrationAPI(EnvironmentID);
                if (integrationAPI != null)
                {
                    string URL = Convert.ToString(integrationAPI.domain_SystemHealth);
                    string Appid = Convert.ToString(integrationAPI.Appid);
                    string AppKey = Convert.ToString(integrationAPI.HealthMeasurementKey);

                    string token = AES.EncryptAES(Appid, AppKey);
                    curl = URL + "api/Monitoring/GetReportByTransactionKey";

                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, null, "", null, null);
                    
                    StartTime = DateTime.Now;

                    List<ListTransactionErrors> Response = new List<ListTransactionErrors>();
                    curl = curl + "?requestID=" + CreateRequestID;
                    var dataresponse = await CallDataDIConnect(curl, token, param);
                    if (dataresponse.ResponseStatus ==(int) ResponseStatus.Success)
                    {
                        Response = JsonConvert.DeserializeObject<List<ListTransactionErrors>>(dataresponse.Data);
                        EndTime = DateTime.Now;

                        var ErrorStatus = await GetErrorStatusModels();

                        foreach (var item in Response)
                        {
                            int ErrorStatus_temp = Convert.ToInt32(item.ErrorStatus.Value);
                            item.ErrorStatusString = ErrorStatus.FirstOrDefault(c => c.ErrorStatus == ErrorStatus_temp).Description;
                        }
                    }                    
                    responseData.data = Response;
                    responseData.status = (int)dataresponse.ResponseStatus;
                    responseData.message = dataresponse.Description;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Succeeded, "", StartTime, EndTime);
                }
                else
                {
                    responseData.status = (int)ResponseStatus.Error;
                    responseData.message = "No data config EnvironmentID";
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                }                
                return responseData;
            }
            catch (Exception ex)
            {
                responseData.status = (int)ResponseStatus.Error;
                responseData.message = ex.Message;
                EndTime = DateTime.Now;
                await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                return responseData;
            }
        }
        public async Task<ResponseData> GetViewLogs(GetViewLogsModel param, int EnvironmentID)
        {
            ResponseData responseData = new ResponseData();
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            Guid CreateRequestID = Guid.NewGuid();
            string curl = "";
            try
            {
                IntegrationAPIModel integrationAPI = await GetTransactionIntegrationAPI(EnvironmentID);
                if (integrationAPI != null)
                {
                    string URL = Convert.ToString(integrationAPI.domain_SystemHealth);
                    string Appid = Convert.ToString(integrationAPI.Appid);
                    string AppKey = Convert.ToString(integrationAPI.HealthMeasurementKey);

                    string token = AES.EncryptAES(Appid, AppKey);
                    curl = URL + "api/Monitoring/GetViewLogs";

                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, null, "", null, null);
                    StartTime = DateTime.Now;

                    List<ListViewLog> Response = new List<ListViewLog>();
                    curl = curl + "?requestID=" + CreateRequestID;
                    var dataresponse = await CallDataDIConnect(curl, token, param);
                    if (dataresponse.ResponseStatus == (int)ResponseStatus.Success)
                    {
                        Response = JsonConvert.DeserializeObject<List<ListViewLog>>(dataresponse.Data);
                    }                    
                    responseData.data = Response;
                    responseData.status = (int)ResponseStatus.Success;
                    responseData.message = null;
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Succeeded, "", StartTime, EndTime);
                }
                else
                {
                    responseData.status = (int)ResponseStatus.Error;
                    responseData.message = "No data config EnvironmentID";
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                }                
                return responseData;
            }
            catch (Exception ex)
            {
                responseData.status = (int)ResponseStatus.Error;
                responseData.message = ex.Message;
                EndTime = DateTime.Now;
                await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                return responseData;
            }
        }
        public async Task<ResponseData> ViewCIPConfiguration(CIPConfig param, int EnvironmentID)
        {
            ResponseData responseData = new ResponseData();
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            Guid CreateRequestID = Guid.NewGuid();
            string curl = "";
            try
            {
                IntegrationAPIModel integrationAPI = await GetTransactionIntegrationAPI(EnvironmentID);
                if (integrationAPI != null)
                {
                    string URL = Convert.ToString(integrationAPI.domain_SystemHealth);
                    string Appid = Convert.ToString(integrationAPI.Appid);
                    string AppKey = Convert.ToString(integrationAPI.HealthMeasurementKey);

                    string token = AES.EncryptAES(Appid, AppKey);
                    curl = URL + "api/Monitoring/ViewCIPConfiguration";

                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, null, "", null, null);
                    StartTime = DateTime.Now;

                    ListViewCIPConfiguration Response = new ListViewCIPConfiguration();
                    curl = curl + "?requestID=" + CreateRequestID.ToString();
                    var dataresponse = await CallDataDIConnect(curl, token, param);
                    _logger.LogInformation("---ViewCIPConfiguration dataresponse----" + dataresponse);
                    if(dataresponse.ResponseStatus== (int)ResponseStatus.Success)
                    {
                        Response = JsonConvert.DeserializeObject<ListViewCIPConfiguration>(dataresponse.Data);
                        responseData.data = Response;
                        responseData.status = (int)ResponseStatus.Success;
                        responseData.message = null;
                        EndTime = DateTime.Now;
                        await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Succeeded, "", StartTime, EndTime);
                    }
                    else
                    {
                        responseData.data = Response;
                        responseData.status = (int)ResponseStatus.Error;
                        responseData.message = dataresponse.Description;
                        EndTime = DateTime.Now;
                        await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                    }
                }
                else
                {
                    responseData.status = (int)ResponseStatus.Error;
                    responseData.message = "No data config EnvironmentID";
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                }                
                return responseData;
            }
            catch (Exception ex)
            {
                responseData.status = (int)ResponseStatus.Error;
                responseData.message = ex.Message;
                EndTime = DateTime.Now;
                await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                return responseData;
            }
        }
        public async Task<ResponseData> GetContentView(GetContentViewModel param, int EnvironmentID)
        {
            ResponseData responseData = new ResponseData();
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            Guid CreateRequestID = Guid.NewGuid();
            string curl = "";
            try
            {
                IntegrationAPIModel integrationAPI = await GetTransactionIntegrationAPI(EnvironmentID);

                if (integrationAPI != null)
                {
                    string URL = Convert.ToString(integrationAPI.domain_SystemHealth);
                    string Appid = Convert.ToString(integrationAPI.Appid);
                    string AppKey = Convert.ToString(integrationAPI.HealthMeasurementKey);

                    string token = AES.EncryptAES(Appid, AppKey);
                    curl = URL + "api/Monitoring/GetContentView";

                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, null, "", null, null);
                    StartTime = DateTime.Now;

                    ListContentFile Response = new ListContentFile();
                    curl = curl + "?requestID=" + CreateRequestID;
                    var dataresponse = await CallDataDIConnect(curl, token, param);

                    
                    if (dataresponse.ResponseStatus==(int)ResponseStatus.Success)
                    {
                        Response = JsonConvert.DeserializeObject<ListContentFile>(dataresponse.Data);
                        if (Regex.Match(Response.FileContent, @"^[a-zA-Z]:").Success == true)
                        {
                            Response.isFile = 1;
                        }
                        else Response.isFile = 0;
                        responseData.data = Response;
                        responseData.status = (int)ResponseStatus.Success;
                        responseData.message = dataresponse.Description;
                    }
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Succeeded, "", StartTime, EndTime);
                }
                else
                {
                    responseData.status = (int)ResponseStatus.Error;
                    responseData.message = "No data config EnvironmentID";
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                }
                return responseData;
            }
            catch (Exception ex)
            {
                responseData.status = (int)ResponseStatus.Error;
                responseData.message = ex.Message;
                EndTime = DateTime.Now;
                await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, responseData.message, StartTime, EndTime);
                return responseData;
            }
        }
        public async Task<ResponseInfo> DownloadFileContent(int EnvironmentID)
        {

            ResponseInfo responseData = new ResponseInfo();
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            Guid CreateRequestID = Guid.NewGuid();
            string curl = "";
            try
            {
                IntegrationAPIModel integrationAPI = await GetTransactionIntegrationAPI(EnvironmentID);

                if (integrationAPI != null)
                {

                    string URL = Convert.ToString(integrationAPI.domain_SystemHealth);
                    string Appid = Convert.ToString(integrationAPI.Appid);
                    string AppKey = Convert.ToString(integrationAPI.HealthMeasurementKey);
                    string token = AES.EncryptAES(Appid, AppKey);
                    curl = URL + "api/Monitoring/DownloadFileContent";

                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, null, "", null, null);
                    StartTime = DateTime.Now;

                    curl = curl + "?requestID=" + CreateRequestID;
                    responseData.URL = curl;
                    responseData.AppKey = AppKey;
                    responseData.token = token;
                    responseData.Appid = Appid;
                    responseData.RequestID = Convert.ToString(CreateRequestID);
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Succeeded, "", StartTime, EndTime);
                }
                else
                {
                    EndTime = DateTime.Now;
                    await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, "IntegrationAPI is null", StartTime, EndTime);
                }                
                return responseData;

            }
            catch (Exception ex)
            {
                EndTime = DateTime.Now;
                await UpdateDataIntegrationLog(CreateRequestID, EnvironmentID, curl, (int)TransactionLogStatus.Failed, ex.Message, StartTime, EndTime);
                return responseData;
            }
        }
        public async Task<bool> CreateTransDataIntegration(List<TransDataIntegrationModel> param, int? MonitoredStatus)
        {
            try
            {
                foreach (var item in param)
                {
                    var isUpdate = await checkTransactionkey(item.EnviromentId, item.RowID);
                    if (isUpdate == null || isUpdate == "00000000-0000-0000-0000-000000000000")
                    {
                        var TransData = new Trans_Data_Integration()
                        {
                            Id = Guid.NewGuid(),
                            EnviromentId = item.EnviromentId,
                            Note = item.Note,
                            TransactionKey = item.TransactionKey,
                            MonitoredStatus = MonitoredStatus,
                            ReProcess = item.ReProcess,
                            CreateDate = DateTime.Now,
                            RowID = item.RowID,
                        };
                        _monitoringContext.Trans_Data_Integrations.AddRange(TransData);
                    }
                    else
                    {
                        var dtUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Trans_Data_Integrations.FirstOrDefaultAsync(x => x.Id.ToString() == isUpdate),CancellationToken.None);
                        if (MonitoredStatus == null)
                        {
                            MonitoredStatus = dtUpdate.MonitoredStatus;
                        }
                        dtUpdate.Note = item.Note;
                        dtUpdate.MonitoredStatus = MonitoredStatus;
                        dtUpdate.ReProcess = item.ReProcess;
                        dtUpdate.UpdateDate = DateTime.Now;
                        _monitoringContext.Trans_Data_Integrations.UpdateRange(dtUpdate);
                    }
                }
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<string> checkTransactionkey(int? EnviromentID, int? rowID)
        {
            try
            {
                var id = await DBRetryHelper.Default.ExecuteAsync(() => (from trd in _monitoringContext.Trans_Data_Integrations
                                where trd.EnviromentId == EnviromentID && trd.RowID == rowID
                                select trd.Id).FirstOrDefaultAsync(),CancellationToken.None);
                return id.ToString();
            }
            catch (Exception)
            {
                return null;
            }

        }
        private dynamic ReturnValue(dynamic value)
        {
            if (value == null)
            {
                return "null";
            }
            else
            {
                if (value.GetType() == typeof(string))
                {
                    return "N'" + value + "'";
                }
                else
                {
                    return value;
                }

            }
        }

        public async Task<IntegrationAPIModel> GetTransactionIntegrationAPI(int EnvironmentID)
        {
            IntegrationAPIModel Response = new IntegrationAPIModel();
            try
            {
                _logger.LogInformation("---Begin GetTransactionIntegrationAPI ");
                var sysAPI = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Integration_APIs.Where(c => c.IsActive == true && c.NodeType == 1 && c.EnvironmentID == EnvironmentID).FirstOrDefaultAsync(), CancellationToken.None);
                if (sysAPI != null)
                {
                    Response.domain_SystemHealth = Convert.ToString(sysAPI.domain_SystemHealth);
                    Response.Appid = Convert.ToString(sysAPI.Appid);
                    Response.HealthMeasurementKey = Convert.ToString(sysAPI.HealthMeasurementKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            _logger.LogInformation("---End GetTransactionIntegrationAPI----");
            return Response;
        }

        public async Task<Guid> UpdateDataIntegrationLog(Guid RequestID, int EnvironmentID,string curl, int? Status, string Error, DateTime? StartTime, DateTime? EndTime)
        {
            try
            {
                _logger.LogInformation("---Begin UpdateDataIntegrationLog ");
                var transactionBase_Log = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.TransactionBase_Log.Where(c => c.EnvironmentID == EnvironmentID && c.RequestID == RequestID).FirstOrDefaultAsync(), CancellationToken.None);
                if (transactionBase_Log != null)
                {
                    transactionBase_Log.CreatedDate = DateTime.Now;
                    transactionBase_Log.Status = Status;
                    transactionBase_Log.Error_Message = Error;
                    transactionBase_Log.StartTime = StartTime;
                    transactionBase_Log.EndTime = EndTime;
                    _monitoringContext.Entry(transactionBase_Log).State=EntityState.Modified;
                }
                else
                {
                    transactionBase_Log = new TransactionBase_Log();
                    transactionBase_Log.ID = Guid.NewGuid();
                    transactionBase_Log.EnvironmentID = EnvironmentID;
                    transactionBase_Log.URL = curl;
                    transactionBase_Log.RequestID = RequestID;
                    transactionBase_Log.CreatedDate = DateTime.Now;
                    _monitoringContext.TransactionBase_Log.Add(transactionBase_Log);
                }
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(), CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            _logger.LogInformation("---End UpdateDataIntegrationLog----");
            return RequestID;
        }

        public async Task<List<ErrorStatusModel>> GetErrorStatusModels()
        {
            var Response = new List<ErrorStatusModel>();
            try
            {
                _logger.LogInformation("---Begin GetErrorStatusModels ");
               var sys_Errors = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SqlQueryAsync<Sys_ErrorStatus>($@"SELECT ses.ErrorStatus,ses.ErrorName, ses.[Description], ses.GroupCode FROM Sys_ErrorStatus AS ses", CancellationToken.None),CancellationToken.None);
                foreach(var item in sys_Errors)
                {
                    var statusModel = new ErrorStatusModel();
                    statusModel.ErrorStatus = item.ErrorStatus;
                    statusModel.ErrorName = item.ErrorName;
                    statusModel.Description= item.Description;
                    statusModel.GroupCode = item.GroupCode;
                    Response.Add(statusModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            _logger.LogInformation("---End GetErrorStatusModels----");
            return Response;
        }
    }
}
