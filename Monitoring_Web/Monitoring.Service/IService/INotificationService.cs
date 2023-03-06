using Monitoring.Model.Model;
using Monitoring_Common.Common;

namespace Monitoring.Service.IService
{
    public interface INotificationService
    {
        Task<ApiResult<string>> CreateNotification(NotificationAddRequest request);
        Task<PagedResult<NotificationModel>> GetListNotification(NotificationListRequest request);
        Task<ApiResult<string>> UpdateNotification(NotificationEditRequest request);
        Task<NotificationEditRequest> GetNotificationEdit(int id);
        Task<bool> ToggleNotification(ToggleNotificationRequest request);
        Task<bool> DeleteNotification(List<NotificationModel> request);
    }
}
