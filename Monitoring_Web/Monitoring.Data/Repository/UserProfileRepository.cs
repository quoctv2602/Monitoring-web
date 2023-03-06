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
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public UserProfileRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }

        public async Task<List<UserProfileModel>> GetUsers(UserProfileFilterRequestModel filterModel)
        {
            try
            {
                string? email = filterModel.Email;
                int? pageSize = filterModel.PageSize;
                int? pageNumber = filterModel.PageNumber;
                int? userType = filterModel.UserType;
                bool? isDelete = filterModel.IsDelete;
                var userProfileList = await DBRetryHelper.Default.ExecuteAsync(() => (from u in _monitoringContext.Sys_UserProfile
                                                            join g in _monitoringContext.Sys_Group on u.GroupId equals g.GroupId
                                                            into Result
                                                            from g in Result.DefaultIfEmpty()
                                                            where (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email) || u.Email.Contains(email))
                                                            && (userType == null || u.UserType == userType)
                                                            && (isDelete == null || ((u.IsDelete == null ? false : u.IsDelete) == isDelete))
                                                            select new UserProfileModel
                                                            {
                                                                Id = u.Id,
                                                                GroupId = u.GroupId,
                                                                Email = u.Email,
                                                                GroupName = g.GroupName,
                                                                UserType = u.UserType,
                                                                IsDelete = u.IsDelete,
                                                            }).ToListAsync(),CancellationToken.None);
                int totalRows = userProfileList.Count();
                foreach (UserProfileModel c in userProfileList)
                {
                    c.TotalRows = totalRows;
                }
                if (pageSize != null && pageNumber != null)
                {
                    userProfileList = userProfileList.OrderBy(u => u.Id).Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList();
                }
                return userProfileList;
            }
            catch (Exception ex)
            {
                return new List<UserProfileModel>();
            }
        }

        public async Task<UserProfileModel?> GetUserProfile(int id)
        {
            try
            {
                var userProfile = await DBRetryHelper.Default.ExecuteAsync(() => (from u in _monitoringContext.Sys_UserProfile
                                                        join g in _monitoringContext.Sys_Group on u.GroupId equals g.GroupId
                                                        into Result
                                                        from g in Result.DefaultIfEmpty()
                                                        where u.Id == id
                                                        select new UserProfileModel
                                                        {
                                                            Id = u.Id,
                                                            GroupId = u.GroupId,
                                                            Email = u.Email,
                                                            GroupName = g.GroupName,
                                                            UserType = u.UserType,
                                                        }).FirstOrDefaultAsync(), CancellationToken.None);
                return userProfile;
            }
            catch (Exception ex)
            {
                return new UserProfileModel();
            }
        }

        public async Task<int> SaveUserProfile(UserProfileModel saveModel)
        {
            try
            {
                var existUser = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_UserProfile.Where(u => u.Email != null && saveModel.Email != null && u.Email.Trim() == saveModel.Email.Trim() && u.Id != saveModel.Id).FirstOrDefaultAsync(),CancellationToken.None);
                if (existUser != null)
                {
                    return -2;
                }
                Sys_UserProfile saveEntity = new Sys_UserProfile()
                {
                    Id = saveModel.Id,
                    Email = saveModel.Email,
                    UserType = saveModel.UserType,
                    GroupId = saveModel.GroupId
                };
                if (saveModel.Id <= 0)
                {
                    _monitoringContext.Sys_UserProfile.Add(saveEntity);
                }
                else
                {
                    _monitoringContext.Sys_UserProfile.Update(saveEntity);
                }
                var returnResult = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);

                int userId = saveEntity.Id;

                return returnResult;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> DeleteUser(UserProfileModel deleteModel)
        {
            try
            {
                var deleteId = deleteModel.Id;
                var isDelete = deleteModel.IsDelete;
                var deleteUser = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_UserProfile.Where(g => g.Id == deleteId).FirstOrDefaultAsync(),CancellationToken.None);
                if (deleteUser != null)
                {
                    deleteUser.IsDelete = isDelete;
                    _monitoringContext.Sys_UserProfile.Update(deleteUser);
                }
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
