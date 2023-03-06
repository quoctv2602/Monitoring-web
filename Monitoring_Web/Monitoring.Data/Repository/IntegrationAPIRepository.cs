using DiCentral.RetrySupport._6._0.DBHelper;
using Microsoft.EntityFrameworkCore;
using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;

namespace Monitoring.Data.Repository
{

    public class IntegrationAPIRepository : IIntegrationAPIRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public IntegrationAPIRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }
        public async Task<PagedResult<IntegrationAPIModel>> GetListIntegrationAPI(IntegrationAPIRequest request)
        {
            try
            {
                var query = from ns in _monitoringContext.Sys_Integration_APIs
                            join ets in _monitoringContext.Sys_Environments on ns.EnvironmentID equals ets.ID
                            where ns.NodeType==2
                            orderby ns.ID descending
                            select new { ns, ets};
               
                //2. filter
                if (!string.IsNullOrWhiteSpace(request.MachineName))
                {
                    query = query.Where(p => p.ns.MachineName.Contains(request.MachineName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(request.EnvironmentName))
                {
                    query = query.Where(p => p.ets.Name.Contains(request.EnvironmentName.Trim()));
                }
                //3. Paging
                int totalRow = await DBRetryHelper.Default.ExecuteAsync(() =>  query.CountAsync(),CancellationToken.None);

                var data = await DBRetryHelper.Default.ExecuteAsync(() =>  query.OrderByDescending(c=>c.ns.CreateDate).Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new IntegrationAPIModel
                {
                    ID = x.ns.ID,
                    EnvironmentId=x.ns.EnvironmentID,
                    EnvironmentName = x.ets.Name,
                    MachineName = x.ns.MachineName,
                    HealthMeasurementKey = x.ns.HealthMeasurementKey,
                    Appid = x.ns.Appid,
                    domain_SystemHealth = x.ns.domain_SystemHealth,
                    NodeType = x.ns.NodeType,
                    IsActive = x.ns.IsActive,
                    CreateDate = x.ns.CreateDate,
                    IsCheck=false,
                    IsDefaultNode=x.ns.IsDefaultNode??false,
                }).ToListAsync(),CancellationToken.None);

                var pagedResult = new PagedResult<IntegrationAPIModel>()
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
                return new PagedResult<IntegrationAPIModel>();
            }
        }
        public async Task<bool> CreateIntegrationAPI(CreateIntegrationAPIRequest request)
        {
            try
            {
                var IntegrationAPI = new Sys_Integration_API()
                {
                    EnvironmentID = request.EnvironmentID,
                    MachineName = request.MachineName,
                    HealthMeasurementKey = request.HealthMeasurementKey,
                    Appid = request.Appid,
                    domain_SystemHealth = request.domain_SystemHealth,
                    NodeType=request.NodeType,
                    IsActive = request.IsActive,
                    CreateDate = DateTime.Now,
                    ServiceList = request.ServiceList,
                    IsDefaultNode = request.IsDefaultNode,
                };
                 _monitoringContext.Sys_Integration_APIs.Add(IntegrationAPI);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateIntegrationAPI(UpdateIntegrationAPIRequest request)
        {
            try
            {
                var dtUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Integration_APIs.FirstOrDefaultAsync(x => x.ID == request.ID),CancellationToken.None);
                if(dtUpdate == null)return false;
                dtUpdate.HealthMeasurementKey = request.HealthMeasurementKey;
                dtUpdate.Appid = request.Appid;
                dtUpdate.domain_SystemHealth = request.domain_SystemHealth;
                dtUpdate.IsActive = request.IsActive;
                dtUpdate.CreateDate = DateTime.Now;
                dtUpdate.ServiceList = request.ServiceList;
                dtUpdate.IsDefaultNode = request.IsDefaultNode;
                _monitoringContext.Sys_Integration_APIs.Update(dtUpdate);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<IntegrationAPIModelEdit> GetIntegrationAPIEdit(int id)
        {
            try
            {
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Integration_APIs.Where(x=>x.ID==id).Select(x=> new IntegrationAPIModelEdit
                {
                    ID = x.ID,
                    EnvironmentID = x.EnvironmentID,
                    MachineName = x.MachineName,
                    HealthMeasurementKey = x.HealthMeasurementKey,
                    Appid = x.Appid,   
                    domain_SystemHealth=x.domain_SystemHealth,
                    NodeType = x.NodeType,
                    IsActive = x.IsActive,
                    CreateDate = x.CreateDate,
                    ServiceList = x.ServiceList,
                    IsDefaultNode = x.IsDefaultNode,
                }).FirstAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return new IntegrationAPIModelEdit();
            }
        }
        public async Task<int> GetIntegrationAPI(string machineName, int environmentID)
        {
            try
            {
                var integrationId = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Integration_APIs
                                    where c.MachineName == machineName && c.EnvironmentID == environmentID
                                    select c.ID).FirstOrDefaultAsync(),CancellationToken.None);
                if (integrationId == null) { integrationId = 0; }
                return integrationId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<List<Sys_Environment>> GetEnvironment()
        {
            try
            {
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Environments.ToListAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return new List<Sys_Environment>();
            }
        }
        public async Task<string> DeleteIntegrationAPI(List<IntegrationAPIModel> request)
        {
            try
            {
                foreach (var item in request)
                {
                    var integration = await _monitoringContext.Sys_Integration_APIs.FindAsync(item.ID);
                    if (integration == null) { return "Error"; }
                    var exitNodeSetting = (from ns in _monitoringContext.Sys_Node_Settings
                                          where ns.EnvironmentID == integration.EnvironmentID && ns.NodeName == integration.MachineName
                                          select ns.NodeName).FirstOrDefault();
                    if (exitNodeSetting != null) { return exitNodeSetting; }
                    _monitoringContext.Sys_Integration_APIs.Remove(integration);
                }
                 await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return "Success";
            }
            catch (Exception)
            {
                return "Error";
                throw;
            }
        }
        public async Task<List<Sys_NodeType>> GetNodeType()
        {
            try
            {
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_NodeTypes.ToListAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return new List<Sys_NodeType>();
            }
        }
        public async Task<bool> UpdateAPITransaction(UpdateAPITransactionRequest request)
        {
            try
            {
                var dtUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Integration_APIs.FirstOrDefaultAsync(x => x.EnvironmentID == request.EnvironmentId  && x.NodeType== request.NodeType),CancellationToken.None);
                if (dtUpdate == null) return false;
                dtUpdate.HealthMeasurementKey = request.HealthMeasurementKey;
                dtUpdate.Appid = request.Appid;
                dtUpdate.domain_SystemHealth = request.domain_SystemHealth;
                _monitoringContext.Sys_Integration_APIs.Update(dtUpdate);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateIsActiveNodeManagament(int environmentID,int? nodeType,bool isActive)
        {
            try
            {
                var Integration = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Integration_APIs where c.EnvironmentID == environmentID && c.NodeType== nodeType
                                         select c).FirstOrDefaultAsync(),CancellationToken.None);
                    Integration.IsActive = isActive;
                _monitoringContext.Sys_Integration_APIs.Update(Integration);
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None) > 0;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> UpdateIsDefaultNode(int environmentID)
        {
            try
            {
                var dtUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Integration_APIs.FirstOrDefaultAsync(x =>x.EnvironmentID==environmentID && x.IsDefaultNode == true && x.NodeType==2),CancellationToken.None);
                if (dtUpdate == null) return true;
                dtUpdate.IsDefaultNode = false;
                _monitoringContext.Sys_Integration_APIs.Update(dtUpdate);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
