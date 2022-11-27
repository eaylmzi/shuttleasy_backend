using System.Security.Claims;

namespace shuttleasy.Services.UserServices
{
    public class PassengerService : IPassengerService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PassengerService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }



        public string getPassenger()
        {
            var result = string.Empty;
            if(_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            }
            return result;
        }
    }
}
