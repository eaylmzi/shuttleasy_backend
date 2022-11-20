using shuttleasy.DAL.Models;

namespace shuttleasy.JwtToken
{
    public interface IJwtTokenManager
    {
        public string CreateToken(Passenger passenger, IConfiguration _configuration);
    }
}
