using Newtonsoft.Json;

namespace shuttleasy.Services.NotificationServices
{
    public class ResponseModel
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; } = null!;

    }
}
