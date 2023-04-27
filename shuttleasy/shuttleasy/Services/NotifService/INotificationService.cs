using FirebaseAdmin.Messaging;

namespace shuttleasy.Services.NotifService
{
    public interface INotificationService
    {
       // public Task<string> aaa(NotificationModel notificationModel);
        public Task<bool> SendNotificationByTopic(NotificationModelTopic notificationModel);
        public Task<BatchResponse> SendNotificationByToken(NotificationModelToken notificationModelToken);
    }
}

