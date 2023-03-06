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
    public class GroupRepository : IGroupRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public GroupRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }

        public async Task<List<GroupModel>> GetGroups(GroupFilterRequestModel filterModel)
        {
            try
            {
                string? groupName = filterModel.GroupName;
                int? pageSize = filterModel.PageSize;
                int? pageNumber = filterModel.PageNumber;
                var listGroup = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group
                    .Where(g => (string.IsNullOrEmpty(groupName) || string.IsNullOrWhiteSpace(groupName) || g.GroupName.Contains(groupName)))
                    .Select(a => new GroupModel
                    {
                        GroupDescription = a.GroupDescription,
                        GroupId = a.GroupId,
                        GroupName = a.GroupName,
                        IsDefault = a.IsDefault,
                        Members = _monitoringContext.Sys_UserProfile.Where(u => u.GroupId == a.GroupId && u.IsDelete != true).Select(u => new UserProfileModel
                        {
                            UserType = u.UserType,
                            Email = u.Email,
                            GroupId = u.GroupId,
                            GroupName = a.GroupName,
                            Id = u.Id,
                        }).ToList()
                    }).ToListAsync(),CancellationToken.None);
                int totalRows = listGroup.Count();
                foreach (GroupModel group in listGroup)
                {
                    group.TotalRows = totalRows;
                }
                if (pageSize != null && pageNumber != null)
                {
                    listGroup = listGroup.OrderBy(g => g.GroupId).Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList();
                }
                return listGroup;
            }
            catch (Exception ex)
            {
                return new List<GroupModel>();
            }
        }

        public async Task<GroupModel?> GetById(int id)
        {
            try
            {
                var group = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group.Where(g => g.GroupId == id).Select(a => new GroupModel
                {
                    IsDefault = a.IsDefault,
                    GroupDescription = a.GroupDescription,
                    GroupId = a.GroupId,
                    GroupName = a.GroupName,
                    Members = _monitoringContext.Sys_UserProfile.Where(u => u.GroupId == a.GroupId && u.IsDelete != true).Select(u => new UserProfileModel
                    {
                        UserType = u.UserType,
                        Email = u.Email,
                        GroupId = u.GroupId,
                        GroupName = a.GroupName,
                        Id = u.Id,
                    }).ToList()
                }).FirstOrDefaultAsync(),CancellationToken.None);
                return group;
            }
            catch (Exception ex)
            {
                return new GroupModel();
            }
        }

        public async Task<int> SaveGroup(GroupModel saveModel)
        {
            try
            {
                //Check exists group name
                var existGroupName = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group
                    .Where(u => u.GroupName != null && saveModel.GroupName != null && u.GroupName.Trim() == saveModel.GroupName.Trim() && u.GroupId != saveModel.GroupId).FirstOrDefaultAsync(),CancellationToken.None);
                if (existGroupName != null)
                {
                    return -2;
                }
                Sys_Group saveEntity = new Sys_Group()
                {
                    IsDefault = saveModel.IsDefault,
                    GroupDescription = saveModel.GroupDescription?.Trim(),
                    GroupName = saveModel.GroupName?.Trim(),
                    GroupId = saveModel.GroupId,
                };
                if (saveModel.GroupId <= 0)
                {
                    _monitoringContext.Sys_Group.Add(saveEntity);
                }
                else
                {
                    _monitoringContext.Sys_Group.Update(saveEntity);
                }
                var returnResult = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);

                int groupId = saveEntity.GroupId;

                int? groupIdDefault = null;

                var groupDefault = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group.Where(g => g.IsDefault == true).FirstOrDefaultAsync(),CancellationToken.None);
                if (groupDefault != null)
                {
                    groupIdDefault = groupDefault.GroupId;
                }
                if (saveModel.Members != null && saveModel.Members.Count > 0)
                {
                    var listUserTracking = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_UserProfile.Where(u => u.GroupId == groupId).ToListAsync(),CancellationToken.None);
                    listUserTracking.ForEach(u => { u.GroupId = groupIdDefault; });
                    _monitoringContext.Sys_UserProfile.UpdateRange(listUserTracking);
                    await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                    var listSaveModelIds = saveModel.Members.Select(a => a.Id).ToList();
                    var listUserUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_UserProfile.Where(u => listSaveModelIds.Contains(u.Id) == true).ToListAsync(),CancellationToken.None);
                    listUserUpdate.ForEach(u => { u.GroupId = groupId; });
                    _monitoringContext.Sys_UserProfile.UpdateRange(listUserUpdate);
                    await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                }

                return returnResult;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> ChangeDefault(GroupModel changeModel)
        {
            try
            {
                var trackingModel = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group.Where(g => g.GroupId == changeModel.GroupId).FirstOrDefaultAsync(),CancellationToken.None);
                var previousDefaultModel = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group.Where(g => g.IsDefault == true).FirstOrDefaultAsync(),CancellationToken.None);
                if (trackingModel != null)
                {
                    if (previousDefaultModel != null)
                        previousDefaultModel.IsDefault = false;
                    trackingModel.IsDefault = changeModel.IsDefault;
                    _monitoringContext.Sys_Group.Update(trackingModel);
                    return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                }
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> Delete(List<GroupModel> deleteModels)
        {
            try
            {
                var deleteIds = deleteModels.Select(g => g.GroupId).ToList();
                var listUser = await DBRetryHelper.Default.ExecuteAsync(() => (from u in _monitoringContext.Sys_UserProfile
                                                     join g in _monitoringContext.Sys_Group on u.GroupId equals g.GroupId
                                                     where deleteIds.Contains(g.GroupId)
                                                     select u).ToListAsync(),CancellationToken.None);
                if (listUser != null)
                {
                    listUser.ForEach(u => { u.GroupId = null; });
                    _monitoringContext.Sys_UserProfile.UpdateRange(listUser);
                }
                var listGroupDelete = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group.Where(g => deleteIds.Contains(g.GroupId)).ToListAsync(),CancellationToken.None);
                _monitoringContext.Sys_Group.RemoveRange(listGroupDelete);
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
