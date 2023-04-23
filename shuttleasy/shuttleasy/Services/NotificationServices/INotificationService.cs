namespace shuttleasy.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(NotificationModel notificationModel);
    }
}
