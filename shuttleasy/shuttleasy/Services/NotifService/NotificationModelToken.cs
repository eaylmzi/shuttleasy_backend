using Newtonsoft.Json;

namespace shuttleasy.Services.NotifService
{
    public class NotificationModelToken
    {
        [JsonProperty("Token")]
        public List<string> Token { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
