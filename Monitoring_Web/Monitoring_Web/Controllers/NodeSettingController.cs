using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring_Common.Common;
using Monitoring_Web.Filter;
using Monitoring_Web.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static Monitoring_Common.Enum;

namespace Monitoring_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodeSettingController : BaseController
    {
        private readonly INodeSettingService _nodeSettingService;
        private readonly ILogger<NodeSettingController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IIntegrationAPIService _integrationAPIService;
        public NodeSettingController(INodeSettingService nodeSettingService,
            ILogger<NodeSettingController> logger,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper,
            IIntegrationAPIService integrationAPIService)
        {
            this._nodeSettingService = nodeSettingService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            this._integrationAPIService = integrationAPIService;
        }
        [HttpGet]
        [Route("getSysMonitoring")]
        public async Task<ActionResult<List<Sys_Monitoring>>> GetSysMonitoring()
        {
            try
            {
                var list = await _nodeSettingService.GetSysMonitoring();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSysEnvironment")]
        public async Task<ActionResult<NodeSetingsModel>> GetSysEnvironment()
        {
            try
            {
                var list = await _nodeSettingService.GetSysEnvironment();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsSetKPI)]
        [HttpPost]
        [Route("createNodeSetting")]
        public async Task<ActionResult> InsertNodeSetting([FromBody] NodeSettingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (!string.IsNullOrWhiteSpace(request.NotificationEmail))
                {
                    request.NotificationEmail = request.NotificationEmail.Replace(',', ';');
                }
                if (!string.IsNullOrWhiteSpace(request.ReportEmail))
                {
                    request.ReportEmail = request.ReportEmail.Replace(',', ';');
                }
                if(request.NodeType != 2)
                {
                    CreateIntegrationAPIRequest createIntegrationAPIRequest = new CreateIntegrationAPIRequest();
                    createIntegrationAPIRequest.EnvironmentID = request.EnvironmentID;
                    createIntegrationAPIRequest.MachineName = null;
                    createIntegrationAPIRequest.HealthMeasurementKey = request.HealthMeasurementKey;
                    createIntegrationAPIRequest.Appid = request.Appid;
                    createIntegrationAPIRequest.domain_SystemHealth = request.domain_SystemHealth;
                    createIntegrationAPIRequest.NodeType = request.NodeType;
                    createIntegrationAPIRequest.IsActive = true;
                    createIntegrationAPIRequest.ServiceList = null;
                    var dt = await _integrationAPIService.CreateIntegrationAPI(createIntegrationAPIRequest);
                    if (dt.IsSuccessed == false)
                    {
                        return Ok(dt);
                    }
                }
                var result = await _nodeSettingService.CreateNodeSetting(request);
                if (result.IsSuccessed == false)
                {
                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsUpdateKPI)]
        [HttpPost]
        [Route("updateNodeSetting")]
        public async Task<ActionResult> UpdateNodeSetting([FromBody] NodeSettingEditRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (!string.IsNullOrWhiteSpace(request.NotificationEmail))
                {
                    request.NotificationEmail = request.NotificationEmail.Replace(',', ';');
                }
                if (!string.IsNullOrWhiteSpace(request.ReportEmail))
                {
                    request.ReportEmail = request.ReportEmail.Replace(',', ';');
                }
                var nodeType = await _nodeSettingService.GetNodeType(request.ID);
                if(request.NodeType == 1 && nodeType==1)
                {
                    UpdateAPITransactionRequest updateAPITransactionRequest = new UpdateAPITransactionRequest();
                    updateAPITransactionRequest.Appid = request.Appid;
                    updateAPITransactionRequest.domain_SystemHealth = request.domain_SystemHealth;
                    updateAPITransactionRequest.HealthMeasurementKey = request.HealthMeasurementKey;
                    updateAPITransactionRequest.NodeType = request.NodeType;
                    updateAPITransactionRequest.EnvironmentId = request.EnvironmentID;
                    var updateAPI = await _integrationAPIService.UpdateAPITransaction(updateAPITransactionRequest);
                    if(updateAPI.IsSuccessed == false)
                    {
                        return Ok(updateAPI);
                    }
                }
                if (request.NodeType == 1 && nodeType == 2)
                {
                    request.NodeName = null;
                    request.MachineName = null;
                    request.ServiceList = null;
                    CreateIntegrationAPIRequest createIntegrationAPIRequest = new CreateIntegrationAPIRequest();
                    createIntegrationAPIRequest.EnvironmentID = request.EnvironmentID;
                    createIntegrationAPIRequest.MachineName = null;
                    createIntegrationAPIRequest.HealthMeasurementKey = request.HealthMeasurementKey;
                    createIntegrationAPIRequest.Appid = request.Appid;
                    createIntegrationAPIRequest.domain_SystemHealth = request.domain_SystemHealth;
                    createIntegrationAPIRequest.NodeType = request.NodeType;
                    createIntegrationAPIRequest.IsActive = true;
                    createIntegrationAPIRequest.ServiceList = null;
                    var dt = await _integrationAPIService.CreateIntegrationAPI(createIntegrationAPIRequest);
                    if (dt.IsSuccessed == false)
                    {
                        return Ok(dt);
                    }
                }
                var result = await _nodeSettingService.UpdateNodeSetting(request);
                if (result.IsSuccessed == false)
                {
                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("getListSysNodeSetting")]
        public async Task<ActionResult<PagedResult<NodeSettings>>> GetListSysNodeSetting([FromBody] SettingsRequest request)
        {
            try
            {
                var list = await _nodeSettingService.GetListSysNodeSetting(request);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("getSysNodeSetting")]
        public async Task<ActionResult<NodeSettingsEdit>> GetSysNodeSetting([FromBody] int id)
        {
            try
            {
                var list = await _nodeSettingService.GetSysNodeSetting(id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("getSysThresholdRule")]
        public async Task<ActionResult<List<Sys_Threshold_Rule>>> GetSysThresholdRule([FromBody] NodeSettingIdRequest request)
        {
            try
            {
                var list = await _nodeSettingService.GetSysThresholdRule(request.Id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsImportExportKPI)]
        [HttpPost]
        [Route("importFileJson")]
        public async Task<ActionResult> ImportFileJson(IFormFile files)
        {
            if (files != null)
            {
                var file = files;
                string fileNameExtension = (file.FileName.Split("."))[(file.FileName.Split(".")).Length - 1];
                string fileName = "FilejsonUpload_FromMornitoring" + "_" + DateTime.Now.ToString().Replace(":", "").Replace("/", "").Replace(" ", "") + "." + fileNameExtension;

                string folder = _webHostEnvironment.WebRootPath + $@"\uploaded\json";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder)
;
                }
                string filePath = Path.Combine(folder, fileName);
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                var jsonData = System.IO.File.ReadAllText(filePath); //read all the content inside the file
                if (string.IsNullOrWhiteSpace(jsonData)) return null;
                var dataImport = JsonConvert.DeserializeObject<List<NodeSettingsJson>>(jsonData);
                string nodeError = "";
                foreach (var item in dataImport)
                {
                    var itemInsert = _mapper.Map<KPIListExportModel>(item);
                    if(itemInsert.NotificationEmail != null)
                    {
                        var NotificationEmail = itemInsert.NotificationEmail.Length;
                        if (NotificationEmail > 200)
                        {
                            return Ok(new ApiErrorResult<string>("Email To Alert Stopped Service is max length 200"));
                        }
                        string[] emailNoti = itemInsert.NotificationEmail.Replace(',', ';').Split(';');
                        for (int i = 0; i < emailNoti.Length; i++)
                        {
                            var checkmail = EmailValid.EmailIsValid(emailNoti[i]);
                            if (checkmail == false)
                            {
                                return Ok(new ApiErrorResult<string>("Email To Alert Stopped Service is not correct"));
                            }
                        }
                        string[] emailReport = itemInsert.ReportEmail.Replace(',', ';').Split(';');
                        for (int i = 0; i < emailReport.Length; i++)
                        {
                            var checkmail = EmailValid.EmailIsValid(emailReport[i]);
                            if (checkmail == false)
                            {
                                return Ok(new ApiErrorResult<string>("Report Email is not correct"));
                            }
                        }
                    }
                    if (itemInsert.ReportEmail!=null)
                    {
                        var ReportEmail = itemInsert.ReportEmail.Length;
                        if (ReportEmail > 200)
                        {
                            return Ok(new ApiErrorResult<string>("Report Email is max length 200"));
                        }
                    }
                    if (itemInsert.NotificationAlias!=null)
                    {
                        var NotificationAlias = itemInsert.NotificationAlias.Length;
                        if (NotificationAlias > 100)
                        {
                            return Ok(new ApiErrorResult<string>("Email Alias is max length 100"));
                        }
                    }
                    if (itemInsert.ReportAlias != null)
                    {
                        var ReportAlias = itemInsert.ReportAlias.Length;
                        if (ReportAlias > 100)
                        {
                            return Ok(new ApiErrorResult<string>("Report Alias is max length 100"));
                        }
                    }
                    for (int i = 0; i < itemInsert.ListThresholdRule.Count(); i++)
                    {
                        var monitoringtpi = itemInsert.ListThresholdRule[i].MonitoringType;
                        var condition = itemInsert.ListThresholdRule[i].Condition;
                        var threshold = itemInsert.ListThresholdRule[i].Threshold;
                        var thresholdCounter = itemInsert.ListThresholdRule[i].ThresholdCounter;
                        if (monitoringtpi == 0)
                        {
                            return Ok(new ApiErrorResult<string>("KPI is required"));
                        }
                        if (condition == 0 || condition == null)
                        {
                            return Ok(new ApiErrorResult<string>("Condition is required"));
                        }
                        if (threshold <= 0)
                        {
                            return Ok(new ApiErrorResult<string>("Threshold not suitable"));
                        }
                        if (thresholdCounter <= 0)
                        {
                            return Ok(new ApiErrorResult<string>("Threshold Counter not suitable"));
                        }
                        for (int j = i + 1; j < itemInsert.ListThresholdRule.Count(); j++)
                        {
                            var monitoringtpj = itemInsert.ListThresholdRule[j].MonitoringType;
                            if (monitoringtpi == monitoringtpj)
                            {
                                return Ok(new ApiErrorResult<string>("Duplicated KPI"));
                            }
                        }
                    }
                    if (itemInsert.NodeType != 2)
                    {
                        if (string.IsNullOrWhiteSpace(itemInsert.HealthMeasurementKey))
                        {
                            return Ok(new ApiErrorResult<string>("HealthMeasurementKey is required"));
                        }
                        if (string.IsNullOrWhiteSpace(itemInsert.Appid))
                        {
                            return Ok(new ApiErrorResult<string>("Appid is required"));
                        }
                        if (string.IsNullOrWhiteSpace(itemInsert.domain_SystemHealth))
                        {
                            return Ok(new ApiErrorResult<string>("domain_SystemHealth is required"));
                        }
                        CreateIntegrationAPIRequest createIntegrationAPIRequest = new CreateIntegrationAPIRequest();
                        createIntegrationAPIRequest.EnvironmentID = itemInsert.EnvironmentID;
                        createIntegrationAPIRequest.MachineName = null;
                        createIntegrationAPIRequest.HealthMeasurementKey = itemInsert.HealthMeasurementKey;
                        createIntegrationAPIRequest.Appid = itemInsert.Appid;
                        createIntegrationAPIRequest.domain_SystemHealth = itemInsert.domain_SystemHealth;
                        createIntegrationAPIRequest.NodeType = itemInsert.NodeType;
                        createIntegrationAPIRequest.IsActive = true;
                        createIntegrationAPIRequest.ServiceList = null;
                        var dt = await _integrationAPIService.CreateIntegrationAPI(createIntegrationAPIRequest);
                        if (dt.IsSuccessed == false)
                        {
                            return Ok(dt);
                        }
                    }
                   
                    var result = await _nodeSettingService.CreateNodeSettingImport(itemInsert);
                    if (result.IsSuccessed == false)
                    {
                        if (nodeError == "")
                        {
                            nodeError = itemInsert.NodeName;
                        }
                        else
                        {
                            nodeError += ";" + itemInsert.NodeName;
                        }
                    }
                }
                if(nodeError == "")
                {
                    return Ok(new ApiSuccessResult<string>());
                }

                return Ok(new ApiErrorResult<string>(nodeError));
            }
            throw new Exception("Save the node failed on save");
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsImportExportKPI)]
        [HttpPost]
        [Route("exportFileJson")]
        public async Task<ActionResult> ExportFileJson(string nodeSettingId)
        {
            try
            {
                var _data = await _nodeSettingService.GetDataExportJson(nodeSettingId);
                string json = JsonConvert.SerializeObject(_data.ToArray());

                var fileName = "FilejsonExport_FromMornitoring" + "_" + DateTime.Now.ToString().Replace(":", "").Replace("/", "").Replace(" ", "") + ".json";
                var mimeType = "text/plain";
                var fileBytes = Encoding.ASCII.GetBytes(json);
                return new FileContentResult(fileBytes, mimeType)
                {
                    FileDownloadName = fileName
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.kpiSettingsEnableDisableKPI)]
        [HttpPost]
        [Route("updateIsActiveNode")]
        public async Task<IActionResult> UpdateNodeSetting([FromBody] NodeSettings request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (request.NodeType == 1)
                {
                    var dtUpdate = await _integrationAPIService.UpdateIsActiveNodeManagament(request.EnvironmentID, request.NodeType,request.IsActive);
                    if (dtUpdate == false)
                    {
                        return Ok(new ApiErrorResult<string>("Update Enable/ Disable error"));
                    }
                }
                var result = await _nodeSettingService.UpdateIsActiveNode(request);
                if (!result)
                {
                    return Ok(new ApiErrorResult<string>("Update Actived Node Management error"));
                }
                return Ok(new ApiSuccessResult<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }   
    }
}
