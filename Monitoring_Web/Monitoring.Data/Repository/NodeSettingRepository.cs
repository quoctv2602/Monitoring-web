using DiCentral.RetrySupport._6._0.DBHelper;
using Microsoft.EntityFrameworkCore;
using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Monitoring.Data.Repository
{
    public class NodeSettingRepository : INodeSettingRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public NodeSettingRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }
        public async Task<List<Sys_Monitoring>> GetSysMonitoring()
        {
            try
            {
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Monitorings.OrderBy(a=>a.Orders).ToListAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return new List<Sys_Monitoring>();
            }
        }
        public async Task<List<EnvironmentModel>> GetSysEnvironment()
        {
            try
            {
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Environments.Select(x => new EnvironmentModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Comment = x.Comment,
                    ListIntegrationAPI = _monitoringContext.Sys_Integration_APIs.Where(z => z.EnvironmentID == x.ID && z.NodeType==2).Select(y => new IntegrationAPI
                    {
                        ID = y.ID,
                        NodeID = y.EnvironmentID,
                        NodeName = y.MachineName,
                        ServiceList = y.ServiceList,
                        
                    }).ToList()
                }).ToListAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return new List<EnvironmentModel>();
            }
        }
        public async Task<int> CreateNodeSetting(NodeSettingRequest request)
        {
            try
            {
                var nodeSetting = new Sys_Node_Setting()
                {
                    NodeName = request.NodeName,
                    EnvironmentID = request.EnvironmentID,
                    MachineName = request.NodeName,
                    Description = request.Description,
                    ServiceList = request.ServiceList,
                    NotificationEmail = request.NotificationEmail,
                    ReportEmail = request.ReportEmail,
                    NotificationAlias = request.NotificationAlias,
                    ReportAlias = request.ReportAlias,
                    CreateDate = DateTime.Now,
                    IsActive = true,
                    NodeType = request.NodeType,
                };
                _monitoringContext.Sys_Node_Settings.Add(nodeSetting);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return nodeSetting.ID;
            }
            catch (Exception)
            {

                return 0;
            }

        }
        public async Task<bool> CreateThresholdRule(List<ThresholdRuleRequest> request, int id, int environmentID, string nodeName)
        {
            try
            {
                var listAddItem = new List<Sys_Threshold_Rule>();
                foreach (var item in request)
                {
                    var addItem = new Sys_Threshold_Rule()
                    {
                        Node_Setting = id,
                        EnvironmentID = environmentID,
                        MachineName = nodeName,
                        MonitoringType = item.MonitoringType,
                        MonitoringName = item.MonitoringName,
                        Condition = item.Condition,
                        Threshold = item.Threshold,
                        ThresholdCounter = item.ThresholdCounter,
                        CreateDate = DateTime.Now,
                    };
                    listAddItem.Add(addItem);
                }
                _monitoringContext.Sys_Threshold_Rules.AddRange(listAddItem);
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None) > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public async Task<int> GetNodeName(string nodeName, int environmentID)
        {
            try
            {
                var nodeId = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Node_Settings
                                    where c.NodeName == nodeName && c.EnvironmentID == environmentID
                                    select c.ID).FirstOrDefaultAsync(),CancellationToken.None);
                if (nodeId == null) { nodeId = 0; }
                return nodeId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<bool> UpdateNodeSetting(NodeSettingEditRequest request)
        {
            try
            {
                var nodeSetting = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Node_Settings
                                         where c.ID == request.ID
                                         select c).FirstOrDefaultAsync(),CancellationToken.None);
                if (nodeSetting == null)
                { return false; }
                nodeSetting.NodeName = request.NodeName;
                nodeSetting.MachineName= request.NodeName;
                nodeSetting.Description = request.Description;
                nodeSetting.ServiceList = request.ServiceList;
                nodeSetting.NotificationEmail = request.NotificationEmail;
                nodeSetting.ReportEmail = request.ReportEmail;
                nodeSetting.NotificationAlias = request.NotificationAlias;
                nodeSetting.ReportAlias = request.ReportAlias;
                nodeSetting.CreateDate = DateTime.Now;
                nodeSetting.NodeType = request.NodeType;
                _monitoringContext.Sys_Node_Settings.Update(nodeSetting);
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None) > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public async Task<bool> UpdateThresholdRule(List<ThresholdRuleEditRequest> request, int id, int environmentID, string nodeName,int isUpdateNode)
        {
            try
            {
                var listRules = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Threshold_Rules.Where(x => x.Node_Setting == id).ToListAsync(),CancellationToken.None);
                if (listRules.Count != 0 && isUpdateNode==1)
                {
                    foreach (var item in listRules)
                    {
                        item.MachineName = nodeName;
                        _monitoringContext.Sys_Threshold_Rules.Update(item);
                    }
                }
                var removeRule= listRules.Where(x => !request.Select(y=>y.ID).ToList().Contains(x.ID)).ToList();
                if (removeRule != null)
                {
                    _monitoringContext.Sys_Threshold_Rules.RemoveRange(removeRule);
                }
                foreach (var item in request)
                {
                    var itemRule = listRules.Where(x => x.ID == item.ID).FirstOrDefault();

                    if (itemRule == null)
                    {

                        var monitoringName = _monitoringContext.Sys_Monitorings.Where(c => c.ID == item.MonitoringType).Select(c => c.Name).FirstOrDefault();
                        var addItem = new Sys_Threshold_Rule()
                        {
                            Node_Setting = id,
                            EnvironmentID = environmentID,
                            MachineName = nodeName,
                            MonitoringType = item.MonitoringType,
                            MonitoringName = monitoringName,
                            Condition = item.Condition,
                            Threshold = item.Threshold,
                            ThresholdCounter = item.ThresholdCounter,
                            CreateDate = DateTime.Now,
                        };
                        _monitoringContext.Sys_Threshold_Rules.Add(addItem);
                    }
                    else
                    {
                        itemRule.MachineName = nodeName;
                        itemRule.Condition = item.Condition;
                        itemRule.Threshold = item.Threshold;
                        itemRule.ThresholdCounter = item.ThresholdCounter;
                        _monitoringContext.Sys_Threshold_Rules.Update(itemRule);
                    }
                }
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<PagedResult<NodeSettings>> GetListSysNodeSetting(SettingsRequest request)
        {
            try
            {
                var query = from ns in _monitoringContext.Sys_Node_Settings
                            join ets in _monitoringContext.Sys_Environments on ns.EnvironmentID equals ets.ID
                            join ntp in _monitoringContext.Sys_NodeTypes on ns.NodeType.ToString() equals ntp.ID.ToString()
                            orderby ns.ID descending
                            select new { ns, ets, ntp.NodeType };
                //2. filter
                if (!string.IsNullOrWhiteSpace(request.NodeName))
                {
                    query = query.Where(p => p.ns.NodeName.Contains(request.NodeName.Trim()));
                }
                if (request.IsActive != null && request.IsActive != "")
                {
                    bool isActive = false;
                    if (request.IsActive == "1")
                    {
                        isActive = true;
                    }
                    query = query.Where(p => p.ns.IsActive == isActive);
                }
                //3. Paging
                int totalRow = await DBRetryHelper.Default.ExecuteAsync(() => query.CountAsync(),CancellationToken.None);

                var data = await DBRetryHelper.Default.ExecuteAsync(() => query.OrderByDescending(c=>c.ns.CreateDate).Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new NodeSettings
                {
                    ID = x.ns.ID,
                    NodeName = x.ns.NodeName,
                    EnvironmentID = x.ns.EnvironmentID,
                    EnvironmentName = x.ets.Name,
                    MachineName = x.ns.MachineName,
                    Description = x.ns.Description,
                    ServiceList = x.ns.ServiceList,
                    NotificationEmail = x.ns.NotificationEmail,
                    ReportEmail = x.ns.ReportEmail,
                    CreateDate = x.ns.CreateDate,
                    IsActive = x.ns.IsActive,
                    NodeType = x.ns.NodeType,
                    NodeTypeName = x.NodeType,
                    IsCheck = false,
                }).ToListAsync(),CancellationToken.None);

                var pagedResult = new PagedResult<NodeSettings>()
                {
                    TotalRecords = totalRow,
                    PageSize = request.PageSize,
                    PageIndex = request.PageIndex,
                    listItem = data
                };
                return pagedResult;
            }
            catch (Exception ex)
            {
                return new PagedResult<NodeSettings>();
            }
        }
        public async Task<List<Sys_Integration_API>> GetListIntegrationAPI(int id)
        {
            try
            {
                return await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Integration_APIs where c.EnvironmentID == id select c).ToListAsync(),CancellationToken.None);

            }
            catch (Exception ex)
            {
                return new List<Sys_Integration_API>();
            }
        }      
        public async Task<NodeSettingsEdit> GetSysNodeSetting(int id)
        {
            try
            {
                var query = from thr in _monitoringContext.Sys_Threshold_Rules
                            join m in _monitoringContext.Sys_Monitorings on thr.MonitoringType equals m.ID
                            orderby m.Orders ascending
                            select new { thr, m.Unit };
               var checkNodeType = await DBRetryHelper.Default.ExecuteAsync(() => (from ns in _monitoringContext.Sys_Node_Settings
                                    where ns.ID == id
                                    select ns.NodeType).FirstOrDefaultAsync(),CancellationToken.None);
                if (checkNodeType.ToString() == "2")
                {
                    return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Node_Settings.Where(x => x.ID == id).Select(x => new NodeSettingsEdit
                    {
                        ID = x.ID,
                        NodeName = x.NodeName,
                        EnvironmentID = x.EnvironmentID,
                        MachineName = x.MachineName,
                        Description = x.Description,
                        ServiceList = x.ServiceList,
                        NotificationEmail = x.NotificationEmail,
                        ReportEmail = x.ReportEmail,
                        NotificationAlias = x.NotificationAlias,
                        ReportAlias = x.ReportAlias,
                        CreateDate = x.CreateDate,
                        IsActive = x.IsActive,
                        NodeType = x.NodeType,
                        HealthMeasurementKey=null,
                        Appid=null,
                        domain_SystemHealth=null,
                        ListThresholdRuleEdit = query
                    .Where(z => z.thr.Node_Setting == x.ID)
                    .Select(z => new ThresholdRuleEdit
                    {
                        ID = z.thr.ID,
                        Node_Setting = z.thr.Node_Setting,
                        EnvironmentID = z.thr.EnvironmentID,
                        MachineName = z.thr.MachineName,
                        MonitoringType = z.thr.MonitoringType,
                        MonitoringName = z.thr.MonitoringName,
                        Condition = z.thr.Condition,
                        Threshold = z.thr.Threshold,
                        ThresholdCounter = z.thr.ThresholdCounter,
                        CreateDate = z.thr.CreateDate,
                        Unit = z.Unit,
                    }).ToList(),
                    }).FirstAsync(),CancellationToken.None);
                }
                else
                {
                    var data = from ns in _monitoringContext.Sys_Node_Settings
                             join thr in _monitoringContext.Sys_Integration_APIs on ns.EnvironmentID equals thr.EnvironmentID
                             where ns.ID == id && thr.NodeType != 2
                             select new { ns, thr.domain_SystemHealth, thr.Appid, thr.HealthMeasurementKey };
                    return await DBRetryHelper.Default.ExecuteAsync(() => data.Select(x=>new NodeSettingsEdit
                    {
                        ID = x.ns.ID,
                        NodeName = x.ns.NodeName,
                        EnvironmentID = x.ns.EnvironmentID,
                        MachineName = x.ns.MachineName,
                        Description = x.ns.Description,
                        ServiceList = x.ns.ServiceList,
                        NotificationEmail = x.ns.NotificationEmail,
                        ReportEmail = x.ns.ReportEmail,
                        NotificationAlias = x.ns.NotificationAlias,
                        ReportAlias = x.ns.ReportAlias,
                        CreateDate = x.ns.CreateDate,
                        NodeType = x.ns.NodeType,
                        HealthMeasurementKey = x.HealthMeasurementKey,
                        Appid = x.Appid,
                        domain_SystemHealth = x.domain_SystemHealth,
                        ListThresholdRuleEdit = query
                    .Where(z => z.thr.Node_Setting == x.ns.ID)
                    .Select(z => new ThresholdRuleEdit
                    {
                        ID = z.thr.ID,
                        Node_Setting = z.thr.Node_Setting,
                        EnvironmentID = z.thr.EnvironmentID,
                        MachineName = z.thr.MachineName,
                        MonitoringType = z.thr.MonitoringType,
                        MonitoringName = z.thr.MonitoringName,
                        Condition = z.thr.Condition,
                        Threshold = z.thr.Threshold,
                        ThresholdCounter = z.thr.ThresholdCounter,
                        CreateDate = z.thr.CreateDate,
                        Unit = z.Unit,
                    }).ToList(),
                    }).FirstAsync(),CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                return new NodeSettingsEdit();
            }
        }
        public async Task<List<Sys_Threshold_Rule>> GetSysThresholdRule(int Node_Setting)
        {
            try
            {
                return await DBRetryHelper.Default.ExecuteAsync(() => (from tr in _monitoringContext.Sys_Threshold_Rules where tr.Node_Setting == Node_Setting select tr).ToListAsync(),CancellationToken.None);

            }
            catch (Exception ex)
            {
                return new List<Sys_Threshold_Rule>();
            }
        }
        public async Task<List<KPIListExportModel>> GetDataExportJson(string id)
        {
            try
            {
                List<KPIListExportModel> listDataExport = new List<KPIListExportModel>();
                string[] arrId=id.Split(',');
                foreach (var item in arrId)
                {
                    var query = from thr in _monitoringContext.Sys_Threshold_Rules
                                join m in _monitoringContext.Sys_Monitorings on thr.MonitoringType equals m.ID
                                orderby m.Orders ascending
                                select new { thr };
                    var checkNodeType = await DBRetryHelper.Default.ExecuteAsync(() => (from ns in _monitoringContext.Sys_Node_Settings
                                               where ns.ID.ToString() == item
                                               select ns.NodeType).FirstOrDefaultAsync(),CancellationToken.None);
                    if (checkNodeType.ToString() == "2")
                    {
                        var dtNodeSystem= await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Node_Settings.Where(x => x.ID.ToString() == item).Select(x => new KPIListExportModel
                        {
                            ID = x.ID,
                            NodeType = x.NodeType,
                            NodeName = x.NodeName,
                            EnvironmentID = x.EnvironmentID,
                            MachineName = x.MachineName,
                            Description = x.Description,
                            ServiceList = x.ServiceList,
                            NotificationEmail = x.NotificationEmail,
                            ReportEmail = x.ReportEmail,
                            NotificationAlias = x.NotificationAlias,
                            ReportAlias = x.ReportAlias,
                            CreateDate = x.CreateDate,
                            HealthMeasurementKey = null,
                            Appid = null,
                            domain_SystemHealth = null,
                            ListThresholdRule = query
                        .Where(z => z.thr.Node_Setting == x.ID)
                        .Select(z => new ThresholdRuleExport
                        {
                            ID = z.thr.ID,
                            Node_Setting = z.thr.Node_Setting,
                            EnvironmentID = z.thr.EnvironmentID,
                            MachineName = z.thr.MachineName,
                            MonitoringType = z.thr.MonitoringType,
                            MonitoringName = z.thr.MonitoringName,
                            Condition = z.thr.Condition,
                            Threshold = z.thr.Threshold,
                            ThresholdCounter = z.thr.ThresholdCounter,
                            CreateDate = z.thr.CreateDate,
                        }).ToList(),
                        }).FirstAsync(),CancellationToken.None);
                        listDataExport.Add(dtNodeSystem);
                    }
                    else
                    {
                        var data = from ns in _monitoringContext.Sys_Node_Settings
                                   join thr in _monitoringContext.Sys_Integration_APIs on ns.EnvironmentID equals thr.EnvironmentID
                                   where ns.ID.ToString() == item && thr.NodeType != 2
                                   select new { ns, thr.domain_SystemHealth, thr.Appid, thr.HealthMeasurementKey };
                        var dtNodeTransaction= await DBRetryHelper.Default.ExecuteAsync(() => data.Select(x => new KPIListExportModel
                        {
                            ID = x.ns.ID,
                            NodeName = x.ns.NodeName,
                            EnvironmentID = x.ns.EnvironmentID,
                            MachineName = x.ns.MachineName,
                            Description = x.ns.Description,
                            ServiceList = x.ns.ServiceList,
                            NotificationEmail = x.ns.NotificationEmail,
                            ReportEmail = x.ns.ReportEmail,
                            NotificationAlias = x.ns.NotificationAlias,
                            ReportAlias = x.ns.ReportAlias,
                            CreateDate = x.ns.CreateDate,
                            NodeType = x.ns.NodeType,
                            HealthMeasurementKey = x.HealthMeasurementKey,
                            Appid = x.Appid,
                            domain_SystemHealth = x.domain_SystemHealth,
                            ListThresholdRule = query
                        .Where(z => z.thr.Node_Setting == x.ns.ID)
                        .Select(z => new ThresholdRuleExport
                        {
                            ID = z.thr.ID,
                            Node_Setting = z.thr.Node_Setting,
                            EnvironmentID = z.thr.EnvironmentID,
                            MachineName = z.thr.MachineName,
                            MonitoringType = z.thr.MonitoringType,
                            MonitoringName = z.thr.MonitoringName,
                            Condition = z.thr.Condition,
                            Threshold = z.thr.Threshold,
                            ThresholdCounter = z.thr.ThresholdCounter,
                            CreateDate = z.thr.CreateDate,
                        }).ToList(),
                        }).FirstAsync(),CancellationToken.None);
                        listDataExport.Add(dtNodeTransaction);
                    }
                }
                return listDataExport;
            }
            catch (Exception ex)
            {
                return new List<KPIListExportModel>();
            }
        }
        public async Task<bool> UpdateIsActiveNode(NodeSettings request)
        {
            try
            {
                    var nodeSetting = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Node_Settings
                                             where c.ID == request.ID
                                             select c).FirstOrDefaultAsync(),CancellationToken.None);
                   
                    nodeSetting.IsActive = request.IsActive;
                    _monitoringContext.Sys_Node_Settings.Update(nodeSetting);
               
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None) > 0;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<int> CheckNodeName(string nodeName, int environmentID,int id)
        {
            try
            {
                var nodeId = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Node_Settings
                                    where c.NodeName == nodeName && c.EnvironmentID == environmentID && c.ID==id
                                    select c.ID).FirstOrDefaultAsync(),CancellationToken.None);
                if (nodeId == null) 
                {
                    nodeId = 0; 
                }
                return nodeId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> checkExitNode(string nodeName, int environmentID)
        {
            try
            {
                var Id = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Integration_APIs
                                    where c.MachineName == nodeName && c.EnvironmentID == environmentID
                                    select c.ID).FirstOrDefaultAsync(),CancellationToken.None);
                if (Id == null) { Id = 0; }
                return Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> CreateNodeSettingImport(KPIListExportModel request)
        {
            try
            {
                var nodeSetting = new Sys_Node_Setting()
                {
                    NodeName = request.NodeName,
                    EnvironmentID = request.EnvironmentID,
                    MachineName = request.NodeName,
                    Description = request.Description,
                    ServiceList = request.ServiceList,
                    NotificationEmail = request.NotificationEmail,
                    ReportEmail = request.ReportEmail,
                    NotificationAlias = request.NotificationAlias,
                    ReportAlias = request.ReportAlias,
                    CreateDate = DateTime.Now,
                    IsActive = request.IsActive,
                    NodeType = request.NodeType,
                };
                _monitoringContext.Sys_Node_Settings.Add(nodeSetting);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return nodeSetting.ID;
            }
            catch (Exception)
            {

                return 0;
            }

        }
        public async Task<bool> CreateThresholdRuleImport(List<ThresholdRuleExport> request, int id, int environmentID, string nodeName)
        {
            try
            {
                var listAddItem = new List<Sys_Threshold_Rule>();
                foreach (var item in request)
                {
                    var addItem = new Sys_Threshold_Rule()
                    {
                        Node_Setting = id,
                        EnvironmentID = environmentID,
                        MachineName = nodeName,
                        MonitoringType = item.MonitoringType,
                        MonitoringName = item.MonitoringName,
                        Condition = item.Condition,
                        Threshold = item.Threshold,
                        ThresholdCounter = item.ThresholdCounter,
                        CreateDate = DateTime.Now,
                    };
                    listAddItem.Add(addItem);
                }
                _monitoringContext.Sys_Threshold_Rules.AddRange(listAddItem);
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None) > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public async Task<int?> GetNodeType(int id)
        {
            try
            {
                var NodeType = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Node_Settings
                                    where c.ID == id 
                                    select c.NodeType).FirstOrDefaultAsync(),CancellationToken.None);
                if (NodeType == null) { NodeType = 2; }
                return NodeType;
            }
            catch (Exception ex)
            {
                return 2;
            }
        }
        public async Task<bool> DeleteIntegrationAPI(int environmentID,int? nodeType)
        {
            try
            {
                    var integration = await DBRetryHelper.Default.ExecuteAsync(() => (from ns in _monitoringContext.Sys_Integration_APIs
                                       where ns.EnvironmentID == environmentID && ns.NodeType == nodeType
                                       select ns).FirstOrDefaultAsync(),CancellationToken.None);
                    if (integration == null) { return false; }
                    _monitoringContext.Sys_Integration_APIs.Remove(integration);
              
                await _monitoringContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
       
    }
}
