using shuttleasy.Models.dto.Credentials.dto;

namespace shuttleasy.Services
{
    public interface IAuthService
    {
        public int GetUserIdFromRequestToken(string? token);
        public string? GetUserRoleFromRequestToken(string? token);
        public string? GetUserTokenFromRequestToken(string? token);
        public UserVerifyingDto GetUserInformation(string? token);
    }
}
