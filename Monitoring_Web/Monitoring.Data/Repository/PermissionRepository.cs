using DiCentral.RetrySupport._6._0.DBHelper;
using Microsoft.EntityFrameworkCore;
using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public PermissionRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }
        public async Task<List<PermissionModel>> GetByGroup(int groupId)
        {
            try
            {
                var listPage = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Pages.ToListAsync(),CancellationToken.None);
                var listAction = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Action.ToListAsync(),CancellationToken.None);

                var permissionByGroup = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_UserAction.Where(ua => ua.GroupId == groupId).ToListAsync(),CancellationToken.None);

                List<PermissionModel> returnPermission = new List<PermissionModel>();
                foreach (Sys_Pages p in listPage)
                {
                    var itemPermission = new PermissionModel();
                    itemPermission.PageId = p.PageId;
                    itemPermission.PageName = p.PageName;
                    itemPermission.Actions = listAction.Where(u => u.PageId == p.PageId).Select(u => new ActionModel
                    {
                        ActionId = u.ActionId,
                        ActionName = u.ActionName,
                        IsSelected = permissionByGroup.Any(a => a.ActionId == u.ActionId && a.PageId == u.PageId)
                    }).ToList();
                    returnPermission.Add(itemPermission);
                }
                return returnPermission;
            }
            catch (Exception ex)
            {
                return new List<PermissionModel>();
            }
        }

        public async Task<int> SavePermission(int groupId, List<PermissionModel> permissions)
        {
            try
            {
                var listPrevious = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_UserAction.Where(u => u.GroupId == groupId).ToListAsync(),CancellationToken.None);
                _monitoringContext.RemoveRange(listPrevious);
                await _monitoringContext.SaveChangesAsync();
                List<Sys_UserAction> saveList = new List<Sys_UserAction>();
                foreach (PermissionModel permission in permissions)
                {
                    foreach (ActionModel action in permission.Actions)
                    {
                        if (action.IsSelected == true)
                        {
                            Sys_UserAction saveItem = new Sys_UserAction()
                            {
                                ActionId = action.ActionId,
                                GroupId = groupId,
                                PageId = permission.PageId
                            };
                            saveList.Add(saveItem);
                        }
                    }
                }
                await _monitoringContext.AddRangeAsync(saveList);
                var returnResult = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return returnResult;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
