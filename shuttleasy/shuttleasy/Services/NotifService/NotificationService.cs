using CorePush.Google;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;


namespace shuttleasy.Services.NotifService
{
    public class NotificationService : INotificationService
    {
        public NotificationService()
        {
            
        }
        public async Task<bool> SendNotificationByTopic(NotificationModelTopic notificationModel)
        {


            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });
            }
 
            Notification not = new Notification();
            not.Title = notificationModel.Title;
            not.Body = notificationModel.Body;
            
            var topic = notificationModel.Topic;
            var message = new Message()
            {
                Notification = not,
                Topic = topic,
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
           
            Console.WriteLine("Successfully sent message: " + response);
            return true;
        }

        public async Task<BatchResponse> SendNotificationByToken(NotificationModelToken notificationModelToken)
        {


            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });
            }

            Notification not = new Notification();
            not.Title = notificationModelToken.Title;
            not.Body = notificationModelToken.Body;

            var message = new MulticastMessage()
            {
                Notification = not,
                Tokens = notificationModelToken.Token,
            };

            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            Console.WriteLine("Successfully sent message: " + response);
            return response;
        }
    }
        /*
          public async Task<string> aaa(NotificationModel notificationModel)
        {
            // The topic name can be optionally prefixed with "/topics/".
            var topic = notificationModel.Topic;

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("appsettings.json")
            });

            // Access Firebase messaging
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = "Title",
                    Body = "Body"
                },
                Topic = "news",
            };
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return response;

        }
        */
        /*
        public async Task<bool> aaa(NotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            FcmSettings settings = new FcmSettings()
            {
                SenderId = _fcmNotificationSetting.SenderId,
                ServerKey = _fcmNotificationSetting.ServerKey
            };
            HttpClient httpClient = new HttpClient();

            string authorizationKey = string.Format("keyy={0}", settings.ServerKey);

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
            httpClient.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
       
            var topic = "aaa";
  




            // See documentation on defining a message payload.
            var message = new Message()
            {
                Data = new Dictionary<string, string>()
                {
                    { "Title", notificationModel.Title },
                    { "Body", notificationModel.Topic },
                },
                Topic = topic,
            };

            var fcm = new FcmSender(settings, httpClient);
            var fcmSendResponse = await fcm.SendAsync(message);
            // string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine("Successfully sent message: " + fcmSendResponse);
            if (fcmSendResponse.IsSuccess())
            {
                return true;
            }
            else
            {
 
                return false;
            }


        }
        */
    }

