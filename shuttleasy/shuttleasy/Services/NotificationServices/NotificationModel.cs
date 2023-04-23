using Newtonsoft.Json;

namespace shuttleasy.Services.NotificationServices
{
    public class NotificationModel
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; } = null!;
        [JsonProperty("isAndroiodDevice")]
        public bool IsAndroiodDevice { get; set; } 
        [JsonProperty("title")]
        public string Title { get; set; } = null!;
        [JsonProperty("body")]
        public string Body { get; set; } = null!;
    }
    public class GoogleNotification
    {
        public class DataPayload
        {
            [JsonProperty("title")]
            public string Title { get; set; } = null!;
            [JsonProperty("body")]
            public string Body { get; set; } = null!;
        }
        [JsonProperty("priority")]
        public string Priority { get; set; } = "high";
        [JsonProperty("data")]
        public DataPayload Data { get; set; } = null!;
        [JsonProperty("notification")]
        public DataPayload Notification { get; set; } = null!;
    }

}
