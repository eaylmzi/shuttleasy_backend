using Microsoft.AspNetCore.Mvc;

using shuttleasy.Services.NotifService;


namespace shuttleasy.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /*
        [Route("send")]
        [HttpPost]
        public async Task<IActionResult> SendNotification(NotificationModel notificationModel)
        {
            var result = await _notificationService.SendNotification(notificationModel);
            return Ok(result);
        }
        */
        [Route("send")]
        [HttpPost]
        public async Task<IActionResult> Send([FromBody]NotificationModelTopic notificationModel)
        {
            var result = await _notificationService.SendNotificationByTopic(notificationModel);
            return Ok(result);
        }
        [Route("send2")]
        [HttpPost]
        public async Task<IActionResult> Send2([FromBody] NotificationModelToken notificationModelToken)
        {
            var result = await _notificationService.SendNotificationByToken(notificationModelToken);
            return Ok(result);
        }

    }
}
