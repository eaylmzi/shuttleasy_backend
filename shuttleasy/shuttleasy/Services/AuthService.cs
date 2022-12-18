using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Asn1.Ocsp;
using shuttleasy.Models.dto.Credentials.dto;
using System.IdentityModel.Tokens.Jwt;

namespace shuttleasy.Services
{
    public class AuthService : IAuthService
    {
        public int GetUserIdFromRequestToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return 0;
            }
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            string user = jwt.Claims.First(c => c.Type == "id").Value;
            int userId = int.Parse(user);
            return userId;
        }
        public string? GetUserRoleFromRequestToken(string? token)
        {
            var requestToken = token;
           if(token == null)
            {
                return null;
            }
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(requestToken);
            string userEmail = jwt.Claims.First(c => c.Type == "role").Value;
            return userEmail;
        }

        public string? GetUserTokenFromRequestToken(string? token)
        {
            return token;
        }
        public UserVerifyingDto GetUserInformation(string? token)
        {
            UserVerifyingDto userVerifyingDto = new UserVerifyingDto();
            var id = GetUserIdFromRequestToken(token);
            userVerifyingDto.Id = id;
            var t = GetUserTokenFromRequestToken(token);
            if(!String.IsNullOrEmpty(t))
            {
                userVerifyingDto.Token = t;
            }
            var r = GetUserRoleFromRequestToken(token);
            if (!String.IsNullOrEmpty(r)) {
                userVerifyingDto.Role = r;
            }
            return userVerifyingDto;
        }
    }
}
