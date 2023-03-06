using DiCentral.RetrySupport._6._0.DBHelper;
using Microsoft.EntityFrameworkCore;
using Monitoring.Data.IRepository;
using Monitoring.Model.Entity;
using Monitoring.Model.Model;

namespace Monitoring.Data.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly MonitoringContext _monitoringContext;
        public NotificationRepository(MonitoringContext monitoringContext)
        {
            _monitoringContext = monitoringContext;
        }
        public async Task<int> CreateNotification(NotificationAddRequest request)
        {
            try
            {
                var notification = new Sys_Notification()
                {
                    Name = request.Name.Trim(),
                    NotificationOption = request.NotificationOption,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    CreatedBy = request.CreatedBy,
                };
                _monitoringContext.Sys_Notifications.Add(notification);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return notification.ID;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<bool> CreateNotificationDetail(NotificationAddRequest request,int id)
        {
            try
            {
                var listAddItem = new List<Sys_Notification_Detail>();
                foreach (var item in request.KPI)
                {
                    var addItem = new Sys_Notification_Detail()
                    {
                        NotificationId = id,
                        KPI = item.MonitoringId,
                        Emails = request.Emails,
                        NotificationAlias = request.NotificationAlias,
                    };
                    listAddItem.Add(addItem);
                }
                _monitoringContext.Sys_Notification_Details.AddRange(listAddItem);
                return await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<PagedResult<NotificationModel>> GetListNotification(NotificationListRequest request)
        {
            try
            {
                var query = from n in _monitoringContext.Sys_Notifications
                            join nd in _monitoringContext.Sys_Notification_Details on n.ID equals nd.NotificationId into ps
                            from nd in ps.DefaultIfEmpty()
                            orderby n.ID descending
                            select new { n.ID,n.Name,n.NotificationOption,n.IsActive, nd.Emails,nd.NotificationAlias };
                query = query.Distinct();
                //2. filter
                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    query = query.Where(p => p.Name.Contains(request.Name.Trim()));
                }
               
                //3. Paging
                int totalRow = await DBRetryHelper.Default.ExecuteAsync(() => query.CountAsync(),CancellationToken.None);
                var queryDetail = from nd in _monitoringContext.Sys_Notification_Details
                                  join m in _monitoringContext.Sys_Monitorings on nd.KPI equals m.ID
                                  select new { nd.NotificationId, m.Name };
                var data = await DBRetryHelper.Default.ExecuteAsync(() => query.OrderByDescending(c => c.ID).Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new NotificationModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    KPI= queryDetail.Where(y => y.NotificationId==x.ID).Select(z => new ListKPI
                    {
                        KPIName = z.Name,
                    }).ToList(),
                    NotificationOption =x.NotificationOption,
                    Emails=x.Emails,
                    NotificationAlias = x.NotificationAlias,
                    IsActive=x.IsActive,
                    IsCheck=false,
                }).ToListAsync(),CancellationToken.None);

                var pagedResult = new PagedResult<NotificationModel>()
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
                return new PagedResult<NotificationModel>();
            }
        }
        public async Task<bool> UpdateNotification(NotificationEditRequest request)
        {
            try
            {
                var dtUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Notifications.FirstOrDefaultAsync(x => x.ID == request.ID),CancellationToken.None);
                if (dtUpdate == null) return false;
                dtUpdate.Name = request.Name;
                dtUpdate.NotificationOption = request.NotificationOption;
                dtUpdate.UpdateDate = DateTime.Now;
                dtUpdate.UpdateBy = request.UpdateBy;
                _monitoringContext.Sys_Notifications.Update(dtUpdate);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateNotificationDetail(NotificationEditRequest request)
        {
            try
            {
                var listDetail = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Notification_Details.Where(x => x.NotificationId == request.ID).ToListAsync(),CancellationToken.None);
               
                var removeRule = listDetail.Where(x => !request.KPI.Select(y => y.ID).ToList().Contains(x.ID)).ToList();
                if (removeRule != null)
                {
                    _monitoringContext.Sys_Notification_Details.RemoveRange(removeRule);
                }
                foreach (var item in request.KPI)
                {
                    var listAddItem = new List<Sys_Notification_Detail>();
                    var listUpdateItem = new List<Sys_Notification_Detail>();
                    if (item.ID==0)
                    {
                        var addItem = new Sys_Notification_Detail()
                        {
                            NotificationId = request.ID,
                            KPI = item.MonitoringId,
                            Emails = request.Emails,
                            NotificationAlias = request.NotificationAlias,
                        };
                        listAddItem.Add(addItem);
                    }
                    else
                    {
                        var dtUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Notification_Details.FirstOrDefaultAsync(x => x.ID == item.ID),CancellationToken.None);
                        if (dtUpdate == null) return false;
                        dtUpdate.Emails = request.Emails;
                        dtUpdate.NotificationAlias = request.NotificationAlias;
                        listUpdateItem.Add(dtUpdate);
                        
                    }
                    _monitoringContext.Sys_Notification_Details.AddRange(listAddItem);
                    _monitoringContext.Sys_Notification_Details.UpdateRange(listUpdateItem);
                }
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<NotificationEditRequest> GetNotificationEdit(int id)
        {
            try
            {
               
                var query = from n in _monitoringContext.Sys_Notifications
                            join nd in _monitoringContext.Sys_Notification_Details on n.ID equals nd.NotificationId into ps
                            from nd in ps.DefaultIfEmpty()
                            where n.ID == id
                            select new { n.ID, n.Name, n.NotificationOption, n.IsActive, nd.Emails, nd.NotificationAlias };
                query = query.Distinct();
               return await DBRetryHelper.Default.ExecuteAsync(() => query.Select(x =>new NotificationEditRequest
                {
                    ID = x.ID,
                    Name = x.Name,
                    NotificationOption = x.NotificationOption, 
                    KPI = _monitoringContext.Sys_Notification_Details.Where(y => y.NotificationId==x.ID).Select(y=> new MonitoringEdit
                    {
                        ID=y.ID,
                        MonitoringId=y.KPI,
                    }).ToList(),
                    Emails = x.Emails,
                    NotificationAlias = x.NotificationAlias,
                    UpdateBy=null,
                }).FirstAsync(),CancellationToken.None);
            }
            catch (Exception ex)
            {
                return new NotificationEditRequest();
            }
        }
        public async Task<bool> ToggleNotification(ToggleNotificationRequest request)
        {
            try
            {
                var dtUpdate = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Notifications.FirstOrDefaultAsync(x => x.ID == request.ID),CancellationToken.None);
                if (dtUpdate == null) return false;
                dtUpdate.IsActive = request.IsActive;
                _monitoringContext.Sys_Notifications.Update(dtUpdate);
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteNotification(List<NotificationModel> request)
        {
            try
            {
                foreach (var item in request)
                {
                    var details = await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.Sys_Notification_Details.Where(x => x.NotificationId == item.ID).Select(x => x).ToListAsync(),CancellationToken.None);
                    if (details == null) { return false; }
                    var notifications = await _monitoringContext.Sys_Notifications.FindAsync(item.ID);
                    if (notifications == null) { return false; }
                    _monitoringContext.Sys_Notification_Details.RemoveRange(details);
                    _monitoringContext.Sys_Notifications.Remove(notifications);
                }
                await DBRetryHelper.Default.ExecuteAsync(() => _monitoringContext.SaveChangesAsync(),CancellationToken.None);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        public async Task<int> CheckNotExitsNotificationName(string name)
        {
            try
            {
                var notificationId = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Notifications
                                    where c.Name.Trim().ToUpper() == name.Trim().ToUpper()
                                            select c.ID).FirstOrDefaultAsync(),CancellationToken.None);
                if (notificationId == null)
                {
                    notificationId = 0;
                }
                return notificationId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> CheckNotExitsNotificationName(int id,string name)
        {
            try
            {
                var notificationId = await DBRetryHelper.Default.ExecuteAsync(() => (from c in _monitoringContext.Sys_Notifications
                                            where c.Name.Trim().ToUpper() == name.Trim().ToUpper() && c.ID==id
                                            select c.ID).FirstOrDefaultAsync(),CancellationToken.None);
                if (notificationId == null)
                {
                    notificationId = 0;
                }
                return notificationId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
