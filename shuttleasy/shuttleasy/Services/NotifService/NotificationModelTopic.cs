using Newtonsoft.Json;

namespace shuttleasy.Services.NotifService
{
    public class NotificationModelTopic
    {

        [JsonProperty("Topic")]
        public string Topic { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
