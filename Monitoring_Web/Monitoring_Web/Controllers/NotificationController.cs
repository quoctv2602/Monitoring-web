using Microsoft.AspNetCore.Mvc;
using Monitoring.Model.Model;
using Monitoring.Service.IService;
using Monitoring_Common.Common;
using static Monitoring_Common.Enum;
using System;
using Monitoring_Web.Filter;

namespace Monitoring_Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : BaseController
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;
        public NotificationController( ILogger<NotificationController> logger, INotificationService notificationService)
        { 
            _logger = logger;
            _notificationService = notificationService;
        }
        [ActionFilter(ActionId = (int)ActionEnum.notificationSettingsManage)]
        [HttpPost]
        [Route("createNotification")]
        public async Task<ActionResult> InsertNotification([FromBody] NotificationAddRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                return BadRequest(ModelState);
                if (!string.IsNullOrWhiteSpace(request.Emails))
                {
                    request.Emails = request.Emails.Replace(',',';');
                }
                var result = await _notificationService.CreateNotification(request);
                if (result.IsSuccessed == false)
                {
                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("getListNotification")]
        public async Task<ActionResult<PagedResult<NotificationModel>>> GetListNotification([FromBody] NotificationListRequest request)
        {
            try
            {
                var list = await _notificationService.GetListNotification(request);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.notificationSettingsManage)]
        [HttpPost]
        [Route("updateNotification")]
        public async Task<ActionResult> UpdateNotification([FromBody] NotificationEditRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (!string.IsNullOrWhiteSpace(request.Emails))
                {
                    request.Emails = request.Emails.Replace(',', ';');
                }
                var result = await _notificationService.UpdateNotification(request);
                if (result.IsSuccessed == false)
                {
                    return Ok(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("getNotificationEdit")]
        public async Task<ActionResult<NotificationEditRequest>> GetNotificationEdit([FromBody] int id)
        {
            try
            {
                var list = await _notificationService.GetNotificationEdit(id);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.notificationSettingsOnOff)]
        [HttpPost]
        [Route("toggleNotification")]
        public async Task<ActionResult> ToggleNotification([FromBody] ToggleNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _notificationService.ToggleNotification(request);
                if (result == false)
                {
                    return Ok(new ApiErrorResult<string>("Toggle Notification error"));
                }
                return Ok(new ApiSuccessResult<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [ActionFilter(ActionId = (int)ActionEnum.notificationSettingsManage)]
        [HttpPost]
        [Route("deleteNotification")]
        public async Task<ActionResult> DeleteNotification([FromBody] List<NotificationModel> request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var result = await _notificationService.DeleteNotification(request);
                if (result == false)
                {
                    return Ok(new ApiErrorResult<string>("Delete Notification error"));
                }
                return Ok(new ApiSuccessResult<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
