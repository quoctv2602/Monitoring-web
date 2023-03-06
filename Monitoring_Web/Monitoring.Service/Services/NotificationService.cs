using Monitoring.Data.IRepository;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring_Common.Common;

namespace Monitoring.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public async Task<ApiResult<string>> CreateNotification(NotificationAddRequest request)
        {
            var id = await _notificationRepository.CheckNotExitsNotificationName(request.Name);
            if (id!=0)
            {
                return new ApiErrorResult<string>("Name already exist");
            }
            var notificationId = await _notificationRepository.CreateNotification(request);
            if (notificationId == 0)
            {
                return new ApiErrorResult<string>("Create notification error");
            }
           
            var result = await _notificationRepository.CreateNotificationDetail(request, notificationId);
            if (result == false)
            {
                return new ApiErrorResult<string>("Create notification detail error");
            }
            return new ApiSuccessResult<string>();
        }
        public async Task<PagedResult<NotificationModel>> GetListNotification(NotificationListRequest request)
        {
            return await _notificationRepository.GetListNotification(request);
        }
        public async Task<ApiResult<string>> UpdateNotification(NotificationEditRequest request)
        {
            var id = await _notificationRepository.CheckNotExitsNotificationName(request.ID,request.Name);
            if (id == 0)
            {
                return new ApiErrorResult<string>("Name already exist");
            }
            var notification = await _notificationRepository.UpdateNotification(request);
            if (notification == false)
            {
                return new ApiErrorResult<string>("Update Notification error");
            }
            var notificationDetail= await _notificationRepository.UpdateNotificationDetail(request);
            if (notificationDetail == false)
            {
                return new ApiErrorResult<string>("Update Notification Detail error");
            }
            return new ApiSuccessResult<string>("Update Notification success");
        }
        public async Task<NotificationEditRequest> GetNotificationEdit(int id)
        {
            return await _notificationRepository.GetNotificationEdit(id);
        }
        public async Task<bool> ToggleNotification(ToggleNotificationRequest request)
        {
            return await _notificationRepository.ToggleNotification(request);
        }
        public async Task<bool> DeleteNotification(List<NotificationModel> request)
        {
            return await _notificationRepository.DeleteNotification(request);
        }
    }
}
