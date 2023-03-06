using Monitoring.Model.Model;

namespace Monitoring.Data.IRepository
{
    public interface  INotificationRepository
    {
        Task<int> CreateNotification(NotificationAddRequest request);
        Task<bool> CreateNotificationDetail(NotificationAddRequest request, int id);
        Task<PagedResult<NotificationModel>> GetListNotification(NotificationListRequest request);
        Task<bool> UpdateNotification(NotificationEditRequest request);
        Task<bool> UpdateNotificationDetail(NotificationEditRequest request);
        Task<NotificationEditRequest> GetNotificationEdit(int id);
        Task<bool> ToggleNotification(ToggleNotificationRequest request);
        Task<bool> DeleteNotification(List<NotificationModel> request);
        Task<int> CheckNotExitsNotificationName(string name);
        Task<int> CheckNotExitsNotificationName(int id, string name);
    }
}
