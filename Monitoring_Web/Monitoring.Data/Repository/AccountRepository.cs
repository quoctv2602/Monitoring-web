using DiCentral.RetrySupport._6._0.DBHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;
using Monitoring_Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Monitoring_Common.Enum;

namespace Monitoring.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MonitoringContext _monitoringContext;
        private readonly ILogger<AccountRepository> _logger;
        public AccountRepository(MonitoringContext monitoringContext, ILogger<AccountRepository> logger)
        {
            _monitoringContext = monitoringContext;
            _logger = logger;
        }

        public async Task<UserLoginModel> OnLoginSuccess(string email)
        {
            try
            {
                var defaultGroup = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Group.Where(u => u.IsDefault == true).FirstOrDefaultAsync(), CancellationToken.None);
                var existUser = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_UserProfile.Where(u => u.Email == email).FirstOrDefaultAsync(), CancellationToken.None);
                bool? IsDelete = false;
                if (existUser == null)
                {
                    var newUser = new Sys_UserProfile()
                    {
                        Email = email,
                        UserType = (int)UserTypeEnum.Normal,
                        GroupId = defaultGroup != null ? defaultGroup.GroupId : null,
                    };
                    await _monitoringContext.Sys_UserProfile.AddAsync(newUser);
                    await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(), CancellationToken.None);
                }
                else
                    IsDelete = existUser.IsDelete;

                List<int> returnPermission = new List<int>();
                List<Sys_Action> actionList = new List<Sys_Action>();
                if (IsDelete == false || IsDelete == null)
                {
                    if (existUser?.UserType == (int)UserTypeEnum.Admin)
                    {
                        actionList = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Action.ToListAsync(), CancellationToken.None);
                    }
                    else
                    {
                        if (defaultGroup != null)
                        {
                            actionList = await DBRetryHelper.Default.ExecuteAsync(() => (from ua in _monitoringContext.Sys_UserAction
                                                               join p in _monitoringContext.Sys_Pages on ua.PageId equals p.PageId
                                                               join a in _monitoringContext.Sys_Action on new { ua.PageId, ua.ActionId } equals new { a.PageId, a.ActionId }
                                                               where ua.GroupId == defaultGroup.GroupId
                                                               select new Sys_Action
                                                               {
                                                                   ActionId = ua.ActionId,
                                                                   PageId = ua.PageId,
                                                                   ActionName = a.ActionName
                                                               }).ToListAsync(), CancellationToken.None);
                        }
                    }
                }
                returnPermission = actionList.Select(a => a.ActionId).ToList();
                return new UserLoginModel()
                {
                    DefaultPage = CommonSetting.DefaultPage,
                    Email = email,
                    Permission = returnPermission,
                    IsDelete = IsDelete
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return new UserLoginModel();
        }
    }
}
