using shuttleasy.DAL.Models;

namespace shuttleasy.JwtToken
{
    public interface IJwtTokenManager
    {
        public string CreateToken(Passenger passenger, string role, IConfiguration _configuration);
        public string CreateToken(CompanyWorker companyWorker, string role, IConfiguration _configuration);
        public bool validateToken(string token, IConfiguration _configuration);
    }
}
