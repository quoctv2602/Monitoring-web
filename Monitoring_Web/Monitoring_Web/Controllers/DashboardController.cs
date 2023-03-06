using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Monitoring.Data.IRepository;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring.Service.Services;
using Monitoring_Common;
using Monitoring_Web.Filter;
using Monitoring_Web.HubConfig;
using Monitoring_Web.TimerFeatures;
using Newtonsoft.Json;
using static Monitoring_Common.Enum;

namespace Monitoring_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : BaseController
    {
        private readonly IHubContext<ChartHub> _hub;
        private readonly TimerManager _timer;
        private readonly ILogger<DashboardController> _logger;
        private readonly IDashboardService _dashboardService;
        private readonly IConfiguration _configuration;
        //private readonly IDistributedCache _distributedCache;

        public DashboardController(IHubContext<ChartHub> hub, TimerManager timer, ILogger<DashboardController> logger, IDashboardService dashboardService, IConfiguration configuration/*, IDistributedCache distributedCache*/)
        {
            _hub = hub;
            _timer = timer;
            _logger = logger;
            _dashboardService = dashboardService;
            _configuration = configuration;
            //_distributedCache = distributedCache;
        }
        [ActionFilter(ActionId = (int)ActionEnum.dashboardViewSystemBased)]
        [HttpPost]
        [Route("SystemHealth")]
        public async Task<IActionResult> SystemHealth([FromBody] DashboardRequest Param)
        {
            try
            {
                
                var dashboardType = Param.DashboardType;
                var systemHealthRequest = Param.SystemHealthRequest;
                var transactionRequest = Param.TransactionRequest;
                var connectionId = Param.ConnectionId;

                if (ConnectionInstance.ListConnection == null)
                {
                    ConnectionInstance.ListConnection = new List<HubConfig.ConnectionInfo>();
                    var item = new HubConfig.ConnectionInfo();
                    if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.SystemHeath)
                        item.SystemHealthRequest = systemHealthRequest;
                    if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.TransactionBased)
                        item.Transaction = transactionRequest;
                    item.ConnectionId = connectionId;
                    item.DashboardType = dashboardType;
                    ConnectionInstance.ListConnection.Add(item);
                }
                else
                {
                    if (ConnectionInstance.ListConnection.Any(a => a.ConnectionId == connectionId && a.DashboardType == dashboardType))
                    {
                        var item = ConnectionInstance.ListConnection.Find(a => a.ConnectionId == connectionId && a.DashboardType == dashboardType);
                        if (item != null)
                        {
                            if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.SystemHeath)
                                item.SystemHealthRequest = systemHealthRequest;
                            if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.TransactionBased)
                                item.Transaction = transactionRequest;
                        }
                    }
                    else
                    {
                        var item = new HubConfig.ConnectionInfo();
                        if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.SystemHeath)
                            item.SystemHealthRequest = systemHealthRequest;
                        if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.TransactionBased)
                            item.Transaction = transactionRequest;
                        item.DashboardType = dashboardType;
                        item.ConnectionId = Param.ConnectionId;
                        ConnectionInstance.ListConnection.Add(item);
                    }
                }
                if (!_timer.IsTimerStarted)
                {
                    await _timer.PrepareTimer(Param);
                }
                if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.SystemHeath)
                {
                    int topSelect = _configuration.GetValue<int>("TopSelect");
                    if (systemHealthRequest?.monitoringType == (int)Monitoring_Common.Enum.MonitoringType.FreeDisk)
                    {
                        var listData = await _dashboardService.DataDashboardSystemHealth_KPIFreeDisk(systemHealthRequest.nodeSettings, systemHealthRequest.monitoringType, topSelect);
                        return Ok(new { Message = "Request Completed", Data = listData });
                    }
                    else
                    {
                        var listData = await _dashboardService.DataDashboardSystemHealth(systemHealthRequest.nodeSettings, systemHealthRequest.monitoringType, topSelect);
                        return Ok(new { Message = "Request Completed", Data = listData });
                    }
                }
                else
                {
                    if (dashboardType == (int)Monitoring_Common.Enum.DashboardType.TransactionBased)
                    {
                        int environmentID = transactionRequest.EnvironmentID;
                        string cipFlow = transactionRequest.CIPFlow;
                        string lastest = transactionRequest.Lastest;
                        int topSelectTable = _configuration.GetSection("TransactionDashboard").GetValue<int>("TopSelectTransactionStatus");
                        int topSelectPendingGraph = _configuration.GetSection("TransactionDashboard").GetValue<int>("TopSelectPendingTransaction");
                        var dataOfTable = await _dashboardService.DashboardTransaction_Table(environmentID, cipFlow, lastest, topSelectTable);
                        var dataOfColumnChart = await _dashboardService.DashboardTransaction_ColumnChart(environmentID, cipFlow, lastest);
                        var dataOfPendingGraph = await _dashboardService.DashboardTransaction_PendingGraph(environmentID, cipFlow, lastest, topSelectPendingGraph);
                        var thresholdPendingGraph = await _dashboardService.DashboardTransaction_ThresholdPendingGraph(environmentID);
                        var returnData = new DashboardTransactionModel();
                        returnData.TableData = dataOfTable;
                        returnData.ColumnChartData = dataOfColumnChart;
                        returnData.PendingGraphData = dataOfPendingGraph;
                        returnData.ThresholdPendingGraph = thresholdPendingGraph;
                        return Ok(new { Message = "Request Completed", Data = returnData });
                    }
                }
                return Ok(new { Message = "Request Completed", Data = Array.Empty<int>() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.dashboardViewSystemBased)]
        [Route("nodes")]
        [HttpGet]
        public async Task<ActionResult<List<NodeSettingModel>>> ListNodes()
        {
            
            try
            {
                //Random random = new Random();
                //int randomNumber = random.Next();
                //var postsJson = JsonConvert.SerializeObject(randomNumber);
                //await _distributedCache.SetStringAsync("Posts", postsJson);
                _logger.LogInformation("---Begin method ListNodes in DashboardController---");
                var listNode = await _dashboardService.ListNodeSettings();
                _logger.LogInformation("---End method ListNodes in DashboardController--");
                return Ok(listNode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.dashboardViewSystemBased)]
        [Route("ServiceList")]
        [HttpPost]
        public async Task<ActionResult<List<ServiceModel>>> ListServices([FromBody] List<ServiceListRequestModel> param)
        {
           
            try
            {
                //var cachedPostsJson = await _distributedCache.GetStringAsync("Posts");
                _logger.LogInformation("---Begin method ServicesList in DashboardController---");
                var listService = await _dashboardService.ServiceList(param);
                _logger.LogInformation("---End method ServiceList in DashboardController---");
                return Ok(listService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [Route("DurationSlideShowDashboard")]
        [HttpGet]
        public ActionResult<int> DurtaionSlideShowDashboard()
        {
            try
            {
                _logger.LogInformation("---Begin method DurtaionSlideShowDashboard in DashboardController---");
                int durationSlideShowDashboard = _configuration.GetValue<int>("DurationSlideShowDashboard");
                _logger.LogInformation("---End method DurtaionSlideShowDashboard in DashboardController---");
                return Ok(durationSlideShowDashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.dashboardViewTransactionBased)]
        [HttpGet]
        [Route("getSysEnvironment")]
        public async Task<ActionResult<List<EnvironmentModel>>> GetSysEnvironmentByNodeType(int? nodeType)
        {
            try
            {
                var list = await _dashboardService.Environments_ByNodeType(nodeType);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
